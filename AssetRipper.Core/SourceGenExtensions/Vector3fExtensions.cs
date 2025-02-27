﻿using AssetRipper.SourceGenerated.Subclasses.Vector3f;
using System.Numerics;

namespace AssetRipper.Core.SourceGenExtensions
{
	public static class Vector3fExtensions
	{
		public static void Scale(this IVector3f vector, float scalar)
		{
			vector.X *= scalar;
			vector.Y *= scalar;
			vector.Z *= scalar;
		}

		public static void CopyValues(this IVector3f vector, Vector3 source)
		{
			vector.X = source.X;
			vector.Y = source.Y;
			vector.Z = source.Z;
		}

		public static void Reset(this IVector3f vector)
		{
			vector.X = 0;
			vector.Y = 0;
			vector.Z = 0;
		}

		public static Vector3 CastToStruct(this IVector3f vector)
		{
			return new Vector3(vector.X, vector.Y, vector.Z);
		}
	}
}
