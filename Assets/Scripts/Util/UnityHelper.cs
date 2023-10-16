using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Object = UnityEngine.Object;

namespace Util
{
	public static class UnityHelper
	{
		public static void DestroyAllChilds(this GameObject parent)
		{
			if( parent == null )
				return;
		
			for (var i = parent.transform.childCount - 1; i >= 0; --i)
			{
				var child = parent.transform.GetChild(i);

				child.gameObject.SetActive(false);

				child.gameObject.Destroy();
			}
		}

		public static void DestroyAllChildsUsingParam(this GameObject parent, params string[] ignoreName)
		{
			for (var i = parent.transform.childCount - 1; i >= 0; --i)
			{
				var child = parent.transform.GetChild(i);

				bool ignore = false;
				foreach( var n in ignoreName)
				{
					if (child.name.Equals(n))
					{
						ignore = true;
						break;
					}
				}

				if (ignore)
					continue;

				child.gameObject.SetActive(false);

				child.gameObject.Destroy();
			}
		}

		public static void SetLayer(int newLayer, GameObject obj)
		{
			if (obj == null)
				return;

			SetLayer(newLayer, obj.transform);
		}

		public static void SetLayer(int newLayer, Transform tr)
		{
			if (tr == null)
				return;

			tr.gameObject.layer = newLayer;
			for (int n = 0; n < tr.childCount; ++n)
			{
				SetLayer(newLayer, tr.GetChild(n));
			}
		}
		
		public static void Destroy(this GameObject go)
		{
			if (go == null)
				return;

			if (Application.isPlaying)
				GameObject.Destroy(go);
			else
				GameObject.DestroyImmediate(go);
		}

		public static void Destroy<T>(this T go) where T : Component
		{
			if (go == null)
				return;

			if (Application.isPlaying)
				GameObject.Destroy(go);
			else
				GameObject.DestroyImmediate(go);
		}

		public static Transform FindByName(this Transform tr, string name)
		{
			if (tr.name.Equals(name))
				return tr;

			foreach (Transform child in tr)
			{
				var result = child.FindByName(name);

				if (result != null)
					return result;
			}

			return null;
		}
	
		public static void Shuffle<T>(this IList<T> list, System.Random random)
		{
			int n = list.Count;
			while (n > 1)
			{
				n--;
				int k = random.Next(n + 1);
				(list[k], list[n]) = (list[n], list[k]);
			}
		}
	}
}