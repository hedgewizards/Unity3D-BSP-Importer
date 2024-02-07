using System;
using System.IO;
using System.Collections.Generic;
using UnityEngine;
using BSPImporter.Textures;

#if UNITY_EDITOR
using UnityEditor;
#endif
using LibBSP;

namespace BSPImporter
{

    /// <summary>
    /// Class used for importing BSPs at runtime or edit-time.
    /// </summary>
    public partial class BSPLoader
    {

        /// <summary>
        /// Is the game currently running?
        /// </summary>
        public static bool IsRuntime
        {
            get
            {
#if UNITY_EDITOR
                return EditorApplication.isPlaying;
#else
                return true;
#endif
            }
        }

        /// <summary>
        /// The <see cref="Settings"/> to use to load a <see cref="BSP"/>.
        /// </summary>
        public Settings settings;

        private ITextureSource TextureSource;
        private BSP bsp;
        private GameObject root;
        private List<EntityInstance> entityInstances = new List<EntityInstance>();
        private Dictionary<string, List<EntityInstance>> namedEntities = new Dictionary<string, List<EntityInstance>>();
        private Dictionary<string, Material> materialDirectory = new Dictionary<string, Material>();

        public BSPLoader(Settings settings, ITextureSource textureSource = null)
        {
            this.settings = settings;
            TextureSource = textureSource?? BuildDefaultTextureSource();
        }

        /// <summary>
        /// Loads a <see cref="BSP"/> into Unity using the settings in <see cref="settings"/>.
        /// </summary>
        public void LoadBSP()
        {
            if (string.IsNullOrEmpty(settings.path) || !File.Exists(settings.path))
            {
                Debug.LogError("Cannot import " + settings.path + ": The path is invalid.");
                return;
            }
            BSP bsp = new BSP(new FileInfo(settings.path));
            try
            {
                LoadBSP(bsp);
            }
            catch (Exception e)
            {
#if UNITY_EDITOR
                EditorUtility.ClearProgressBar();
#endif
#if UNITY_4_0 || UNITY_4_0_1 || UNITY_4_2 || UNITY_4_3 || UNITY_4_5 || UNITY_4_6 || UNITY_5 || UNITY_5_3_OR_NEWER
                Debug.LogException(e);
#else
                Debug.LogError(e.ToString() + "\nat " + e.StackTrace);
#endif
            }
        }

        /// <summary>
        /// Loads <paramref name="bsp"/> into Unity using the settings in <see cref="settings"/>.
        /// </summary>
        /// <param name="bsp">The <see cref="BSP"/> object to import into Unity</param>
        public void LoadBSP(BSP bsp)
        {
            if (bsp == null)
            {
                Debug.LogError("Cannot import BSP: The object was null.");
                return;
            }
            this.bsp = bsp;

            for (int i = 0; i < bsp.Entities.Count; ++i)
            {
                Entity entity = bsp.Entities[i];
#if UNITY_EDITOR
                if (EditorUtility.DisplayCancelableProgressBar("Importing BSP", entity.ClassName + (!string.IsNullOrEmpty(entity.Name) ? " " + entity.Name : ""), i / (float)bsp.Entities.Count))
                {
                    EditorUtility.ClearProgressBar();
                    return;
                }
#endif
                EntityInstance instance = CreateEntityInstance(entity);
                entityInstances.Add(instance);
                namedEntities[entity.Name].Add(instance);

                int modelNumber = entity.ModelNumber;
                if (modelNumber >= 0)
                {
                    BuildMesh(instance);
                }
                else
                {
                    Vector3 angles = entity.Angles;
                    instance.gameObject.transform.rotation = Quaternion.Euler(-angles.x, angles.y, angles.z);
                }

                instance.gameObject.transform.position = entity.Origin.SwizzleYZ() * settings.scaleFactor;
            }

            root = new GameObject(bsp.MapName);
            foreach (KeyValuePair<string, List<EntityInstance>> pair in namedEntities)
            {
                SetUpEntityHierarchy(pair.Value);
            }

            if (settings.entityCreatedCallback != null)
            {
                foreach (EntityInstance instance in entityInstances)
                {
                    settings.entityCreatedCallback(new EntityCreatedCallbackData(this, instance));
                }
            }

#if UNITY_EDITOR
            if (!IsRuntime)
            {
                if ((settings.assetSavingOptions & AssetSavingOptions.Prefab) > 0)
                {
                    string prefabPath = Path.Combine(Path.Combine("Assets", settings.meshPath), bsp.MapName + ".prefab").Replace('\\', '/');
                    Directory.CreateDirectory(Path.GetDirectoryName(prefabPath));
#if UNITY_2018_3_OR_NEWER
                    PrefabUtility.SaveAsPrefabAssetAndConnect(root, prefabPath, InteractionMode.AutomatedAction);
#elif !UNITY_3_4
                    PrefabUtility.CreatePrefab(prefabPath, root, ReplacePrefabOptions.ConnectToPrefab);
#else
                    UnityEngine.Object newPrefab = EditorUtility.CreateEmptyPrefab(prefabPath);
                    EditorUtility.ReplacePrefab(root, newPrefab, ReplacePrefabOptions.ConnectToPrefab);
#endif
                }
                AssetDatabase.Refresh();
            }
            EditorUtility.ClearProgressBar();
#endif
        }

        /// <summary>
        /// Creates a TextureSource that loads from disk in the provided texturePath, plus warns about some stuff
        /// </summary>
        /// <returns></returns>
        private ITextureSource BuildDefaultTextureSource()
        {
            ITextureSource textureSource;
            bool useAssetDatabase = false;
#if UNITY_EDITOR
            if (settings.texturePath.StartsWith(Application.dataPath))
            {
                settings.texturePath = "Assets/" + settings.texturePath.Substring(Application.dataPath.Length + 1);
                useAssetDatabase = true;
            }
            else if ((settings.assetSavingOptions & AssetSavingOptions.Materials) > 0)
            {
                Debug.LogWarning("Using a texture path outside of Assets will not work with material saving enabled.");
            }
#endif
            textureSource = new DefaultTextureSource(settings.texturePath, useAssetDatabase);
            return textureSource;
        }

        /// <summary>
        /// Creates a <see cref="Material"/> object for <paramref name="textureName"/>, or loads it from Assets
        /// if it already exists at edit-time.
        /// </summary>
        /// <param name="textureName">Name of the <see cref="Texture2D"/> to load.</param>
        public void LoadMaterial(string textureName)
        {
#if UNITY_5 || UNITY_5_3_OR_NEWER
            Shader def = Shader.Find("Standard");
#else
            Shader def = Shader.Find("Diffuse");
#endif
            Shader fallbackShader = Shader.Find("VR/SpatialMapping/Wireframe");

            Texture2D texture = TextureSource.LoadTexture(textureName);

            Material material = null;
            bool materialIsAsset = false;
#if UNITY_EDITOR
            string materialPath = null;
            if (settings.materialPath != null)
            {
                materialPath = Path.Combine(Path.Combine("Assets", settings.materialPath), textureName + ".mat").Replace('\\', '/');
                if (!IsRuntime)
                {
                    material = AssetDatabase.LoadAssetAtPath(materialPath, typeof(Material)) as Material;
                }
            }
            if (material != null)
            {
                materialIsAsset = true;
            }
            else
#endif
            {
                material = new Material(def);
                material.name = textureName;
            }

            if (!materialIsAsset)
            {
                if (texture != null)
                {
                    material.mainTexture = texture;
#if UNITY_5 || UNITY_5_3_OR_NEWER
                    material.SetFloat("_Glossiness", 0);
                    material.SetFloat("_SpecularHighlights", 0f);
                    material.EnableKeyword("_SPECULARHIGHLIGHTS_OFF");
#endif
                }
                else if (fallbackShader != null)
                {
                    material = new Material(fallbackShader);
                }
#if UNITY_EDITOR
                if (!IsRuntime && settings.materialPath != null && (settings.assetSavingOptions & AssetSavingOptions.Materials) > 0)
                {
                    Directory.CreateDirectory(Path.GetDirectoryName(materialPath));
                    AssetDatabase.CreateAsset(material, materialPath);
                }
#endif
            }

            materialDirectory[textureName] = material;
        }

        /// <summary>
        /// Creates an <see cref="EntityInstance"/> for <paramref name="entity"/> and creates a new
        /// <see cref="GameObject"/> for it.
        /// </summary>
        /// <param name="entity">The <see cref="Entity"/> to create an <see cref="EntityInstance"/> for.</param>
        /// <returns>The generated <see cref="EntityInstance"/>.</returns>
        protected EntityInstance CreateEntityInstance(Entity entity)
        {
            // Entity.name guaranteed not to be null, empty string is a valid Dictionary key
            if (!namedEntities.ContainsKey(entity.Name) || namedEntities[entity.Name] == null)
            {
                namedEntities[entity.Name] = new List<EntityInstance>();
            }

            EntityInstance instance = new EntityInstance()
            {
                entity = entity,
                gameObject = new GameObject(entity.ClassName + (!string.IsNullOrEmpty(entity.Name) ? " " + entity.Name : string.Empty))
            };

            return instance;
        }

        /// <summary>
        /// Sets up the hierarchy for all <see cref="GameObject"/>s in <paramref name="instances"/> according
        /// to the hierarchy in the <see cref="BSP"/>. Currently only applies to Source engine.
        /// </summary>
        /// <param name="instances">A <see cref="List{EntityInstance}"/> with all <see cref="EntityInstance"/> objects.</param>
        protected void SetUpEntityHierarchy(List<EntityInstance> instances)
        {
            foreach (EntityInstance instance in instances)
            {
                SetUpEntityHierarchy(instance);
            }
        }

        /// <summary>
        /// Finds the <see cref="EntityInstance"/> corresponding to the 'parentname' in <paramref name="instance"/>'s <see cref="Entity"/>
        /// and set the <see cref="GameObject"/> int <paramref name="instance"/> as a child of the parent's <see cref="GameObject"/>.
        /// </summary>
        /// <param name="instance"><see cref="EntityInstance"/> to find the parent's <see cref="EntityInstance"/> for.</param>
        protected void SetUpEntityHierarchy(EntityInstance instance)
        {
            if (!instance.entity.ContainsKey("parentname"))
            {
                instance.gameObject.transform.parent = root.transform;
                return;
            }

            if (namedEntities.ContainsKey(instance.entity["parentname"]))
            {
                if (namedEntities[instance.entity["parentname"]].Count > 1)
                {
                    Debug.LogWarning(string.Format("Entity \"{0}\" claims to have parent \"{1}\" but more than one matching entity exists.",
                        instance.gameObject.name,
                        instance.entity["parentname"]), instance.gameObject);
                }
                instance.gameObject.transform.parent = namedEntities[instance.entity["parentname"]][0].gameObject.transform;
            }
            else
            {
                Debug.LogWarning(string.Format("Entity \"{0}\" claims to have parent \"{1}\" but no such entity exists.",
                    instance.gameObject.name,
                    instance.entity["parentname"]), instance.gameObject);
            }
        }

        /// <summary>
        /// Builds all <see cref="Mesh"/> objects for the <see cref="Entity"/> in <paramref name="instance"/> instance,
        /// combines them if necessary using <see cref="settings"/>.meshCombineOptions and adds the meshes to the
        /// <see cref="GameObject"/> in <paramref name="instance"/>.
        /// </summary>
        /// <param name="instance">The <see cref="EntityInstance"/> to build <see cref="Mesh"/>es for.</param>
        protected void BuildMesh(EntityInstance instance)
        {
            int modelNumber = instance.entity.ModelNumber;
            Model model = bsp.Models[modelNumber];
            Dictionary<string, List<Mesh>> textureMeshMap = new Dictionary<string, List<Mesh>>();
            GameObject gameObject = instance.gameObject;

            List<Face> faces = bsp.GetFacesInModel(model);
            int i = 0;
            for (i = 0; i < faces.Count; ++i)
            {
                Face face = faces[i];
                if (face.NumEdgeIndices <= 0 && face.NumVertices <= 0)
                {
                    continue;
                }

                int textureIndex = bsp.GetTextureIndex(face);
                string textureName = "";
                if (textureIndex >= 0)
                {
                    LibBSP.Texture texture = bsp.Textures[textureIndex];
                    textureName = LibBSP.Texture.SanitizeName(texture.Name, bsp.MapType);

                    if (!textureName.StartsWith("tools/", StringComparison.InvariantCultureIgnoreCase))
                    {
                        if (!textureMeshMap.ContainsKey(textureName) || textureMeshMap[textureName] == null)
                        {
                            textureMeshMap[textureName] = new List<Mesh>();
                        }

                        textureMeshMap[textureName].Add(CreateFaceMesh(face, textureName));
                    }
                }
            }

            if (modelNumber == 0)
            {
                if (bsp.LODTerrains != null)
                {
                    foreach (LODTerrain lodTerrain in bsp.LODTerrains)
                    {
                        if (lodTerrain.TextureIndex >= 0)
                        {
                            LibBSP.Texture texture = bsp.Textures[lodTerrain.TextureIndex];
                            string textureName = texture.Name;

                            if (!textureMeshMap.ContainsKey(textureName) || textureMeshMap[textureName] == null)
                            {
                                textureMeshMap[textureName] = new List<Mesh>();
                            }

                            textureMeshMap[textureName].Add(CreateLoDTerrainMesh(lodTerrain, textureName));
                        }
                    }
                }
            }

            if (settings.meshCombineOptions != MeshCombineOptions.None)
            {
                Mesh[] textureMeshes = new Mesh[textureMeshMap.Count];
                Material[] materials = new Material[textureMeshes.Length];
                i = 0;
                foreach (KeyValuePair<string, List<Mesh>> pair in textureMeshMap)
                {
                    textureMeshes[i] = MeshUtils.CombineAllMeshes(pair.Value.ToArray(), true, false);
                    if (textureMeshes[i].vertices.Length > 0)
                    {
                        if (materialDirectory.ContainsKey(pair.Key))
                        {
                            materials[i] = materialDirectory[pair.Key];
                        }
                        if (settings.meshCombineOptions == MeshCombineOptions.PerMaterial)
                        {
                            GameObject textureGameObject = new GameObject(pair.Key);
                            textureGameObject.transform.parent = gameObject.transform;
                            textureGameObject.transform.localPosition = Vector3.zero;
                            textureMeshes[i].Scale(settings.scaleFactor);
                            if (textureMeshes[i].normals.Length == 0 || textureMeshes[i].normals[0] == Vector3.zero)
                            {
                                textureMeshes[i].RecalculateNormals();
                            }

                            textureMeshes[i].AddToGameObject(new Material[] { materials[i] }, textureGameObject);
#if UNITY_EDITOR
                            Unwrapping.GenerateSecondaryUVSet(textureMeshes[i]);

                            if (!IsRuntime && (settings.assetSavingOptions & AssetSavingOptions.Meshes) > 0)
                            {
                                string meshPath = Path.Combine(Path.Combine(Path.Combine("Assets", settings.meshPath), bsp.MapName), "mesh_" + textureMeshes[i].GetHashCode() + ".asset").Replace('\\', '/');
                                Directory.CreateDirectory(Path.GetDirectoryName(meshPath));
                                AssetDatabase.CreateAsset(textureMeshes[i], meshPath);
                            }
#endif
                        }
                        ++i;
                    }
                }

                if (settings.meshCombineOptions != MeshCombineOptions.PerMaterial)
                {
                    Mesh mesh = MeshUtils.CombineAllMeshes(textureMeshes, false, false);
                    if (mesh.vertices.Length > 0)
                    {
                        mesh.TransformVertices(gameObject.transform.localToWorldMatrix);
                        mesh.Scale(settings.scaleFactor);
                        if (mesh.normals.Length == 0 || mesh.normals[0] == Vector3.zero)
                        {
                            mesh.RecalculateNormals();
                        }

                        mesh.AddToGameObject(materials, gameObject);
#if UNITY_EDITOR
                        Unwrapping.GenerateSecondaryUVSet(mesh);

                        if (!IsRuntime && (settings.assetSavingOptions & AssetSavingOptions.Meshes) > 0)
                        {
                            string meshPath = Path.Combine(Path.Combine(Path.Combine("Assets", settings.meshPath), bsp.MapName), "mesh_" + mesh.GetHashCode() + ".asset").Replace('\\', '/');
                            Directory.CreateDirectory(Path.GetDirectoryName(meshPath));
                            AssetDatabase.CreateAsset(mesh, meshPath);
                        }
#endif
                    }
                }
            }
            else
            {
                i = 0;
                foreach (KeyValuePair<string, List<Mesh>> pair in textureMeshMap)
                {
                    GameObject textureGameObject = new GameObject(pair.Key);
                    textureGameObject.transform.parent = gameObject.transform;
                    textureGameObject.transform.localPosition = Vector3.zero;
                    Material material = materialDirectory[pair.Key];
                    foreach (Mesh mesh in pair.Value)
                    {
                        if (mesh.vertices.Length > 0)
                        {
                            GameObject faceGameObject = new GameObject("Face");
                            faceGameObject.transform.parent = textureGameObject.transform;
                            faceGameObject.transform.localPosition = Vector3.zero;
                            mesh.Scale(settings.scaleFactor);
                            if (mesh.normals.Length == 0 || mesh.normals[0] == Vector3.zero)
                            {
                                mesh.RecalculateNormals();
                            }

                            mesh.AddToGameObject(new Material[] { material }, faceGameObject);
#if UNITY_EDITOR
                            Unwrapping.GenerateSecondaryUVSet(mesh);

                            if (!IsRuntime && (settings.assetSavingOptions & AssetSavingOptions.Meshes) > 0)
                            {
                                string meshPath = Path.Combine(Path.Combine(Path.Combine("Assets", settings.meshPath), bsp.MapName), "mesh_" + mesh.GetHashCode() + ".asset").Replace('\\', '/');
                                Directory.CreateDirectory(Path.GetDirectoryName(meshPath));
                                AssetDatabase.CreateAsset(mesh, meshPath);
                            }
#endif
                        }
                    }
                    ++i;
                }
            }

        }

        /// <summary>
        /// Creates a <see cref="Mesh"/> appropriate for <paramref name="face"/>.
        /// </summary>
        /// <param name="face">The <see cref="Face"/> to create a <see cref="Mesh"/> for.</param>
        /// <param name="textureName">The name of the texture/shader applied to the <see cref="Face"/>.</param>
        /// <returns>The <see cref="Mesh"/> generated for <paramref name="face"/>.</returns>
        protected Mesh CreateFaceMesh(Face face, string textureName)
        {
            Vector2 dims;
            if (!materialDirectory.ContainsKey(textureName))
            {
                LoadMaterial(textureName);
            }
            if (materialDirectory[textureName].HasProperty("_MainTex") && materialDirectory[textureName].mainTexture != null)
            {
                dims = new Vector2(materialDirectory[textureName].mainTexture.width, materialDirectory[textureName].mainTexture.height);
            }
            else
            {
                dims = new Vector2(128, 128);
            }

            Mesh mesh;
            if (face.DisplacementIndex >= 0)
            {
                mesh = MeshUtils.CreateDisplacementMesh(bsp, face, dims);
            }
            else
            {
                mesh = MeshUtils.CreateFaceMesh(bsp, face, dims, settings.curveTessellationLevel);
            }

            return mesh;
        }

        /// <summary>
        /// Creates a <see cref="Mesh"/> appropriate for <paramref name="lodTerrain"/>.
        /// </summary>
        /// <param name="lodTerrain">The <see cref="LODTerrain"/> to create a <see cref="Mesh"/> for.</param>
        /// <param name="textureName">The name of the texture/shader applied to the <see cref="LODTerrain"/>.</param>
        /// <returns>The <see cref="Mesh"/> generated for <paramref name="lodTerrain"/>.</returns>
        protected Mesh CreateLoDTerrainMesh(LODTerrain lodTerrain, string textureName)
        {
            if (!materialDirectory.ContainsKey(textureName))
            {
                LoadMaterial(textureName);
            }

            return MeshUtils.CreateMoHAATerrainMesh(bsp, lodTerrain);
        }
    }
}
