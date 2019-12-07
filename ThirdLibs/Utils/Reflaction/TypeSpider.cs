using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Reflection;

namespace BlackJack.Utils
{
	public sealed class TypeSpider
	{
		/// <summary>
		/// Finds all types match predicater in solution.
		/// </summary>
		/// <returns>The matched types in solution.</returns>
		/// <param name="predicater">Predicater.</param>
		public static IEnumerable<Type> FindTypesInSolution(Func<Type, bool> predicater, String nameInclude="")
		{
			// ���ڻ���Assembly::GetExportedTypes�᷵��������ͬ��Type��������һ�㿼�ǣ�ʹ��Set���˷���Type. -- ����
			ISet<string> typesReturned = new HashSet<string>();

			ISet<string> assembliesVisited = new HashSet<string>();
			Queue<Assembly> assembliesToVisit = new Queue<Assembly>();

			// Start with the .exe assembly.
			Assembly entryAssembly = Assembly.GetEntryAssembly();
            if (entryAssembly == null) yield break;
			assembliesToVisit.Enqueue(entryAssembly);

			// BFS (Breadth-first search)
			while (assembliesToVisit.Count > 0)
			{
				Assembly assemblyVisiting = assembliesToVisit.Dequeue();
				assembliesVisited.Add(assemblyVisiting.FullName);

				// Enqueue referenced assemblies.
				foreach (AssemblyName an in assemblyVisiting.GetReferencedAssemblies())
				{
                    //�ų������ֲ�����Ҫ���AssemblyName
                    if (!String.IsNullOrEmpty(nameInclude) && nameInclude.IndexOf(an.Name, 0) == -1) continue;
					
					//Debug.WriteLine("Visiting={0}", a.FullName);
                    if (!assembliesVisited.Contains(an.FullName))
                    {
                        Assembly a = Assembly.Load(an);
                        assembliesToVisit.Enqueue(a);
                    }
				}

				foreach (Type type in assemblyVisiting.GetExportedTypes())
				{
					if (predicater(type) && typesReturned.Add(type.FullName))
						yield return type;
				}
			}
		}
	}
}

