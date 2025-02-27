﻿using AssetRipper.Core.SourceGenExtensions;
using AssetRipper.Numerics;
using AssetRipper.SourceGenerated.Subclasses.CompressedMesh;
using System;
using System.Numerics;
using System.Runtime.InteropServices;

namespace AssetRipper.Tests
{
	internal class CompressedMeshTests
	{
		private const int VertexCount = 20;
		private static readonly Random random = new Random(57089);
		private static readonly Vector3[] vectors = MakeUnitVectors(VertexCount);
		private static readonly Vector4[] tangents = MakeTangents(22);
		private static readonly Matrix4x4[] matrices = MakeMatrices(11);
		private static readonly uint[] integers = MakeUInts(24);
		private static readonly Vector2[] uv0 = MakeUV(VertexCount);
		private static readonly Vector2[] uv1 = MakeUV(VertexCount);

		private static Vector3[] MakeUnitVectors(int count)
		{
			Vector3[] result = new Vector3[count];
			for (int i = 0; i < count; i++)
			{
				float x = random.NextSingle();
				float y = random.NextSingle();
				float z = random.NextSingle();
				result[i] = Vector3.Normalize(new Vector3(x, y, z));
			}
			return result;
		}

		private static Vector4[] MakeTangents(int count)
		{
			Vector3[] unitVectors = MakeUnitVectors(count);
			Vector4[] result = new Vector4[unitVectors.Length];
			for (int i = 0; i < unitVectors.Length; i++)
			{
				float w = random.NextSingle() < 0.5f ? -1f : 1f;
				result[i] = new Vector4(unitVectors[i], w);
			}
			return result;
		}

		private static Vector2[] MakeUV(int count)
		{
			Vector2[] result = new Vector2[count];
			for (int i = 0; i < count; i++)
			{
				result[i] = new Vector2(random.NextSingle(), random.NextSingle());
			}
			return result;
		}

		private static uint[] MakeUInts(int count)
		{
			uint[] result = new uint[count];
			for (int i = 0; i < count; i++)
			{
				result[i] = unchecked((uint)random.Next());
			}
			return result;
		}

		private static Matrix4x4[] MakeMatrices(int count)
		{
			Matrix4x4[] result = new Matrix4x4[count];
			for (int i = 0; i < count; i++)
			{
				result[i] = new Matrix4x4(
					random.NextSingle(),
					random.NextSingle(),
					random.NextSingle(),
					random.NextSingle(),
					random.NextSingle(),
					random.NextSingle(),
					random.NextSingle(),
					random.NextSingle(),
					random.NextSingle(),
					random.NextSingle(),
					random.NextSingle(),
					random.NextSingle(),
					random.NextSingle(),
					random.NextSingle(),
					random.NextSingle(),
					random.NextSingle());
			}
			return result;
		}

		[Test]
		public void VertexAssignmentSymmetry()
		{
			CompressedMesh_5_0_0_f4 compressedMesh = new();
			compressedMesh.SetVertices(vectors);
			Vector3[] unpackedValues = compressedMesh.GetVertices();
			AreAlmostEqual(vectors, unpackedValues, 0.000001f);
		}

		[Test]
		public void NormalAssignmentSymmetry()
		{
			CompressedMesh_5_0_0_f4 compressedMesh = new();
			compressedMesh.SetNormals(vectors);
			Vector3[] unpackedValues = compressedMesh.GetNormals();
			AreAlmostEqual(vectors, unpackedValues, 0.00001f);
			//Note: this symmetry only happens because the vectors are already normalized.
			//This test would (and should) fail if non-normalized vectors are use.
		}

		[Test]
		public void TangentAssignmentSymmetry()
		{
			CompressedMesh_5_0_0_f4 compressedMesh = new();
			compressedMesh.SetTangents(tangents);
			Vector4[] unpackedValues = compressedMesh.GetTangents();
			AreAlmostEqual(tangents, unpackedValues, 0.00001f);
			//Note: this symmetry only happens because the vectors are already normalized.
			//This test would (and should) fail if non-normalized vectors are use.
		}

		[Test]
		public void FloatColorsNormalAssignmentSymmetry()
		{
			CompressedMesh_5_0_0_f4 compressedMesh = new();
			//These are technically invalid colors since they have values outside [0,1] but it doesn't matter for the test.
			ReadOnlySpan<ColorFloat> colors = MemoryMarshal.Cast<Vector4, ColorFloat>(tangents);
			compressedMesh.SetFloatColors(colors);
			ColorFloat[] unpackedValues = compressedMesh.GetFloatColors();
			AreAlmostEqual(MemoryMarshal.Cast<ColorFloat, Vector4>(colors), MemoryMarshal.Cast<ColorFloat, Vector4>(unpackedValues), 0.00001f);
		}

		[Test]
		public void BindPoseAssignmentSymmetry()
		{
			CompressedMesh_3_4_0_f5 compressedMesh = new();//BindPoses only exists on versions before Unity 5.
			compressedMesh.SetBindPoses(matrices);
			Matrix4x4[] unpackedValues = compressedMesh.GetBindPoses();
			AreAlmostEqual(matrices, unpackedValues, 0.000001f);
		}

		[Test]
		public void TriangleAssignmentSymmetry()
		{
			CompressedMesh_5_0_0_f4 compressedMesh = new();
			compressedMesh.SetTriangles(integers);
			uint[] unpackedValues = compressedMesh.GetTriangles();
			Assert.AreEqual(integers, unpackedValues);
		}

		[Test]
		public void OldUV()
		{
			CompressedMesh_3_4_0_f5 compressedMesh = new();//UV is structured differently on versions before Unity 5.
			compressedMesh.SetVertices(vectors);//Need to set the correct vertex count by filling the vertex buffer.
			compressedMesh.SetUV(uv0, uv1, null, null, null, null, null, null);
			compressedMesh.GetUV(out Vector2[]? unpackedUV0, out Vector2[]? unpackedUV1, out Vector2[]? unpackedUV2, out Vector2[]? unpackedUV3, out Vector2[]? unpackedUV4, out Vector2[]? unpackedUV5, out Vector2[]? unpackedUV6, out Vector2[]? unpackedUV7);
			Assert.Multiple(() =>
			{
				//These UV channels did not exist on versions before Unity 5.
				Assert.That(unpackedUV2, Is.EqualTo(null));
				Assert.That(unpackedUV3, Is.EqualTo(null));
				Assert.That(unpackedUV4, Is.EqualTo(null));
				Assert.That(unpackedUV5, Is.EqualTo(null));
				Assert.That(unpackedUV6, Is.EqualTo(null));
				Assert.That(unpackedUV7, Is.EqualTo(null));
			});
			AreAlmostEqual(uv0, unpackedUV0, 0.000001f);
			AreAlmostEqual(uv1, unpackedUV1, 0.000001f);
		}

		[Test]
		public void NewUVWith2Channels()
		{
			CompressedMesh_5_0_0_f4 compressedMesh = new();//UV is structured differently on versions before Unity 5.
			compressedMesh.SetVertices(vectors);//Need to set the correct vertex count by filling the vertex buffer.
			compressedMesh.SetUV(uv0, uv1, null, null, null, null, null, null);
			compressedMesh.GetUV(out Vector2[]? unpackedUV0, out Vector2[]? unpackedUV1, out Vector2[]? unpackedUV2, out Vector2[]? unpackedUV3, out Vector2[]? unpackedUV4, out Vector2[]? unpackedUV5, out Vector2[]? unpackedUV6, out Vector2[]? unpackedUV7);
			Assert.Multiple(() =>
			{
				Assert.That(unpackedUV0, Is.Not.EqualTo(null), () => "UV0");
				Assert.That(unpackedUV1, Is.Not.EqualTo(null), () => "UV1");
				Assert.That(unpackedUV2, Is.EqualTo(null), () => "UV2");
				Assert.That(unpackedUV3, Is.EqualTo(null), () => "UV3");
				Assert.That(unpackedUV4, Is.EqualTo(null), () => "UV4");
				Assert.That(unpackedUV5, Is.EqualTo(null), () => "UV5");
				Assert.That(unpackedUV6, Is.EqualTo(null), () => "UV6");
				Assert.That(unpackedUV7, Is.EqualTo(null), () => "UV7");
			});
			AreAlmostEqual(uv0, unpackedUV0, 0.000001f);
			AreAlmostEqual(uv1, unpackedUV1, 0.000001f);
		}

		[Test]
		public void NewUVWith4Channels()
		{
			CompressedMesh_5_0_0_f4 compressedMesh = new();//UV only supports more channels after Unity 5.
			compressedMesh.SetVertices(vectors);//Need to set the correct vertex count by filling the vertex buffer.
			compressedMesh.SetUV(uv0, uv1, null, uv1, null, null, uv0, null);
			Assert.That(compressedMesh.UVInfo, Is.GreaterThan(0));
			Assert.That(compressedMesh.UV.NumItems, Is.EqualTo(2 * VertexCount * 4));

			compressedMesh.GetUV(out Vector2[]? unpackedUV0, out Vector2[]? unpackedUV1, out Vector2[]? unpackedUV2, out Vector2[]? unpackedUV3, out Vector2[]? unpackedUV4, out Vector2[]? unpackedUV5, out Vector2[]? unpackedUV6, out Vector2[]? unpackedUV7);

			Assert.Multiple(() =>
			{
				Assert.That(unpackedUV0, Is.Not.EqualTo(null), () => "UV0");
				Assert.That(unpackedUV1, Is.Not.EqualTo(null), () => "UV1");
				Assert.That(unpackedUV2, Is.EqualTo(null), () => "UV2");
				Assert.That(unpackedUV3, Is.Not.EqualTo(null), () => "UV3");
				Assert.That(unpackedUV4, Is.EqualTo(null), () => "UV4");
				Assert.That(unpackedUV5, Is.EqualTo(null), () => "UV5");
				Assert.That(unpackedUV6, Is.Not.EqualTo(null), () => "UV6");
				Assert.That(unpackedUV7, Is.EqualTo(null), () => "UV7");
			});
			AreAlmostEqual(uv0, unpackedUV0, 0.000001f);
			AreAlmostEqual(uv1, unpackedUV1, 0.000001f);
			AreAlmostEqual(uv1, unpackedUV3, 0.000001f);
			AreAlmostEqual(uv0, unpackedUV6, 0.000001f);
		}

		private static void AreAlmostEqual(ReadOnlySpan<Vector2> expected, ReadOnlySpan<Vector2> actual, float maxDeviation)
		{
			if (expected.Length != actual.Length)
			{
				Assert.Fail($"Lengths were inequal.\nExpected: {expected.Length}\nBut was: {actual.Length}");
			}

			for (int i = 0; i < expected.Length; i++)
			{
				if (Vector2.Distance(expected[i], actual[i]) > maxDeviation)
				{
					Assert.Fail($"Values significantly differ at index {i}\nExpected: {expected[i]}\nBut was: {actual[i]}");
				}
			}
		}

		private static void AreAlmostEqual(ReadOnlySpan<Vector3> expected, ReadOnlySpan<Vector3> actual, float maxDeviation)
		{
			if (expected.Length != actual.Length)
			{
				Assert.Fail($"Lengths were inequal.\nExpected: {expected.Length}\nBut was: {actual.Length}");
			}

			for (int i = 0; i < expected.Length; i++)
			{
				if (Vector3.Distance(expected[i], actual[i]) > maxDeviation)
				{
					Assert.Fail($"Values significantly differ at index {i}\nExpected: {expected[i]}\nBut was: {actual[i]}");
				}
			}
		}

		private static void AreAlmostEqual(ReadOnlySpan<Vector4> expected, ReadOnlySpan<Vector4> actual, float maxDeviation)
		{
			if (expected.Length != actual.Length)
			{
				Assert.Fail($"Lengths were inequal.\nExpected: {expected.Length}\nBut was: {actual.Length}");
			}

			for (int i = 0; i < expected.Length; i++)
			{
				if (Vector4.Distance(expected[i], actual[i]) > maxDeviation)
				{
					Assert.Fail($"Values significantly differ at index {i}\nExpected: {expected[i]}\nBut was: {actual[i]}");
				}
			}
		}

		private static void AreAlmostEqual(ReadOnlySpan<float> expected, ReadOnlySpan<float> actual, float maxDeviation)
		{
			if (expected.Length != actual.Length)
			{
				Assert.Fail($"Lengths were inequal.\nExpected: {expected.Length}\nBut was: {actual.Length}");
			}

			for (int i = 0; i < expected.Length; i++)
			{
				if (MathF.Abs(expected[i] - actual[i]) > maxDeviation)
				{
					Assert.Fail($"Values significantly differ at index {i}\nExpected: {expected[i]}\nBut was: {actual[i]}");
				}
			}
		}

		private static void AreAlmostEqual(ReadOnlySpan<Matrix4x4> expected, ReadOnlySpan<Matrix4x4> actual, float maxDeviation)
		{
			AreAlmostEqual(MemoryMarshal.Cast<Matrix4x4, float>(expected), MemoryMarshal.Cast<Matrix4x4, float>(actual), maxDeviation);
		}
	}
}
