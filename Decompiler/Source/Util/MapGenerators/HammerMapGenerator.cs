using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;
using System.Globalization;

using LibBSP;

namespace Decompiler {
	/// <summary>
	/// A class that takes an <see cref="Entities"/> object and can convert it into a <c>string</c>,
	/// to output to a file.
	/// </summary>
	public class HammerMapGenerator {

		private Job _master;

		private Entities _entities;
		private IFormatProvider format = CultureInfo.CreateSpecificCulture("en-US");

		private int _nextID = 0;

		/// <summary>
		/// Creates a new instance of a <see cref="HammerMapGenerator"/> object that will operate on "<paramref name="from"/>".
		/// </summary>
		/// <param name="from">The <see cref="Entities"/> object to output to a <c>string</c>.</param>
		/// <param name="master">The parent <see cref="Job"/> object for this instance.</param>
		public HammerMapGenerator(Entities from, Job master) {
			this._entities = from;
			this._master = master;
		}

		/// <summary>
		/// Parses the <see cref="Entities"/> object pointed to by this object into a <c>string</c>, to output to a file.
		/// </summary>
		/// <returns>A <c>string</c> representation of the <see cref="Entities"/> pointed to by this object.</returns>
		public string ParseMap() {
			// This initial buffer is probably too small (512kb) but should minimize the amount of allocations needed.
			StringBuilder sb = new StringBuilder(524288);
			for (int i = 0; i < _entities.Count; ++i) {
				++_nextID;
				ParseEntity(_entities[i], sb);
			}
			return sb.ToString();
		}

		/// <summary>
		/// Process the data in an <see cref="Entity"/> into the passed <see cref="StringBuilder"/>.
		/// </summary>
		/// <param name="entity">The <see cref="Entity"/> to process.</param>
		/// <param name="sb">A <see cref="StringBuilder"/> object to append processed data from <paramref name="entity"/> to.</param>
		private void ParseEntity(Entity entity, StringBuilder sb) {
			if (entity.ValueIs("classname", "worldspawn")) {
				sb.Append("world\r\n{\r\n");
			} else {
				sb.Append("entity\r\n{\r\n");
			}
			foreach (KeyValuePair<string, string> kvp in entity) {
				sb.Append("\t\"")
				.Append(kvp.Key)
				.Append("\" \"")
				.Append(kvp.Value)
				.Append("\"\r\n");
			}
			sb.Append("\t\"id\" \"")
			.Append(_nextID)
			.Append("\"\r\n");
			if (entity.connections.Any()) {
				sb.Append("\tconnections\r\n\t{\r\n");
				foreach (Entity.EntityConnection connection in entity.connections) {
					sb.Append("\t\t\"")
					.Append(connection.name)
					.Append("\" \"")
					.Append(connection.target)
					.Append(",")
					.Append(connection.action)
					.Append(",")
					.Append(connection.param)
					.Append(",")
					.Append(connection.delay.ToString("###0.######", format))
					.Append(",")
					.Append(connection.fireOnce);
					if (connection.unknown0 != "" || connection.unknown1 != "") {
						sb.Append(",")
						.Append(connection.unknown0)
						.Append(",")
						.Append(connection.unknown1);
					}
					sb.Append("\"\r\n");
				}
				sb.Append("\t}\r\n");
			}
			for (int i = 0; i < entity.brushes.Count; ++i) {
				++_nextID;
				ParseBrush(entity.brushes[i], sb);
			}
			sb.Append("}\r\n");
		}

		/// <summary>
		/// Process the data in a <see cref="MAPBrush"/> into the passed <see cref="StringBuilder"/>.
		/// </summary>
		/// <param name="brush">The <see cref="MAPBrush"/> to process.</param>
		/// <param name="sb">A <see cref="StringBuilder"/> object to append processed data from <paramref name="brush"/> to.</param>
		private void ParseBrush(MAPBrush brush, StringBuilder sb) {
			if (brush.patch != null || brush.terrain != null) {
				return;
			}
			if (brush.sides.Count < 4) {
				// Can't create a brush with less than 4 sides
				_master.Print("WARNING: Tried to create brush from " + brush.sides.Count + " sides!");
				return;
			}
			sb.Append("\tsolid\r\n\t{\r\n\t\t\"id\" \"")
			.Append(_nextID)
			.Append("\"\r\n");
			foreach (MAPBrushSide brushSide in brush.sides) {
				++_nextID;
				ParseBrushSide(brushSide, sb);
			}
			sb.Append("\t}\r\n");
		}

		/// <summary>
		/// Process the data in a <see cref="MAPBrushSide"/> into the passed <see cref="StringBuilder"/>.
		/// </summary>
		/// <param name="brushside">The <see cref="MAPBrushSide"/> to process.</param>
		/// <param name="sb">A <see cref="StringBuilder"/> object to append processed data from <paramref name="brushside"/> to.</param>
		private void ParseBrushSide(MAPBrushSide brushside, StringBuilder sb) {
			sb.Append("\t\tside\r\n\t\t{\r\n\t\t\t\"id\" \"")
			.Append(_nextID)
			.Append("\"\r\n\t\t\t\"plane\" \"(")
			.Append(brushside.vertices[0].x.ToString("###0.######", format))
			.Append(" ")
			.Append(brushside.vertices[0].y.ToString("###0.######", format))
			.Append(" ")
			.Append(brushside.vertices[0].z.ToString("###0.######", format))
			.Append(") (")
			.Append(brushside.vertices[1].x.ToString("###0.######", format))
			.Append(" ")
			.Append(brushside.vertices[1].y.ToString("###0.######", format))
			.Append(" ")
			.Append(brushside.vertices[1].z.ToString("###0.######", format))
			.Append(") (")
			.Append(brushside.vertices[2].x.ToString("###0.######", format))
			.Append(" ")
			.Append(brushside.vertices[2].y.ToString("###0.######", format))
			.Append(" ")
			.Append(brushside.vertices[2].z.ToString("###0.######", format))
			.Append(")\"\r\n\t\t\t\"material\" \"")
			.Append(brushside.texture)
			.Append("\"\r\n\t\t\t\"uaxis\" \"[")
			.Append(brushside.textureS.x.ToString("###0.######", format))
			.Append(" ")
			.Append(brushside.textureS.y.ToString("###0.######", format))
			.Append(" ")
			.Append(brushside.textureS.z.ToString("###0.######", format))
			.Append(" ")
			.Append(brushside.textureShiftS.ToString("###0.######", format))
			.Append("] ")
			.Append(brushside.texScaleX.ToString("###0.####", format))
			.Append("\"\r\n\t\t\t\"vaxis\" \"[")
			.Append(brushside.textureT.x.ToString("###0.######", format))
			.Append(" ")
			.Append(brushside.textureT.y.ToString("###0.######", format))
			.Append(" ")
			.Append(brushside.textureT.z.ToString("###0.######", format))
			.Append(" ")
			.Append(brushside.textureShiftT.ToString("###0.######", format))
			.Append("] ")
			.Append(brushside.texScaleY.ToString("###0.####", format))
			.Append("\"\r\n\t\t\t\"rotation\" \"")
			.Append(brushside.texRot.ToString("###0.####", format))
			.Append("\"\r\n\t\t\t\"lightmapscale\" \"")
			.Append(brushside.lgtScale.ToString("###0.####", format))
			.Append("\"\r\n\t\t\t\"smoothing_groups\" \"0\"\r\n");
			if (brushside.displacement != null) {
				ParseDisplacement(brushside.displacement, sb);
			}
			sb.Append("\t\t}\r\n");
		}

		private void ParseDisplacement(MAPDisplacement displacement, StringBuilder sb) {
			sb.Append("\t\t\tdispinfo\r\n\t\t\t{\r\n\t\t\t\t\"power\" \"")
			.Append(displacement.power)
			.Append("\"\r\n\t\t\t\t\"startposition\" \"[")
			.Append(displacement.start.x.ToString("###0.######", format))
			.Append(" ")
			.Append(displacement.start.y.ToString("###0.######", format))
			.Append(" ")
			.Append(displacement.start.z.ToString("###0.######", format))
			.Append("]\"\r\n\t\t\t\t\"elevation\" \"0\"\r\n\t\t\t\t\"subdiv\" \"0\"\r\n\t\t\t\tnormals\r\n\t\t\t\t{\r\n");
			for (int i = 0; i < System.Math.Pow(2, displacement.power) + 1; ++i) {
				sb.Append("\t\t\t\t\t\"row")
				.Append(i)
				.Append("\" \"");
				for (int j = 0; j < System.Math.Pow(2, displacement.power) + 1; ++j) {
					if (j > 0) {
						sb.Append(" ");
					}
					sb.Append(displacement.normals[i][j].x.ToString("###0.######", format));
					sb.Append(" ");
					sb.Append(displacement.normals[i][j].y.ToString("###0.######", format));
					sb.Append(" ");
					sb.Append(displacement.normals[i][j].z.ToString("###0.######", format));
				}
				sb.Append("\"\r\n");
			}
			sb.Append("\t\t\t\t}\r\n\t\t\t\tdistances\r\n\t\t\t\t{\r\n");
			for (int i = 0; i < System.Math.Pow(2, displacement.power) + 1; ++i) {
				sb.Append("\t\t\t\t\t\"row")
				.Append(i)
				.Append("\" \"");
				for (int j = 0; j < System.Math.Pow(2, displacement.power) + 1; ++j) {
					if (j > 0) {
						sb.Append(" ");
					}
					sb.Append(displacement.distances[i][j].ToString("###0.####", format));
				}
				sb.Append("\"\r\n");
			}
			sb.Append("\t\t\t\t}\r\n\t\t\t\talphas\r\n\t\t\t\t{\r\n");
			for (int i = 0; i < System.Math.Pow(2, displacement.power) + 1; ++i) {
				sb.Append("\t\t\t\t\t\"row")
				.Append(i)
				.Append("\" \"");
				for (int j = 0; j < System.Math.Pow(2, displacement.power) + 1; ++j) {
					if (j > 0) {
						sb.Append(" ");
					}
					sb.Append(displacement.alphas[i][j].ToString(format));
				}
				sb.Append("\"\r\n");
			}
			sb.Append("\t\t\t\t}\r\n\t\t\t\ttriangle_tags\r\n\t\t\t\t{\r\n\t\t\t\t}\r\n\t\t\t\ttriangle_tags\r\n\t\t\t\t{\r\n\t\t\t\t\t\"10\" \"-1 -1 -1 -1 -1 -1 -1 -1 -1 -1\"\r\n\t\t\t\t}\r\n\t\t\t}\r\n");
		}
	}
}
