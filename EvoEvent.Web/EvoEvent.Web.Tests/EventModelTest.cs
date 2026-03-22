using System.Collections;

namespace EvoEvent.Web.Tests
{
	public class EventModelTest : IEnumerable<object[]>
	{
		public IEnumerator<object[]> GetEnumerator()
		{
			for (int i = 1; i < 22; i++)
			{
				if (i % 2 == 0)
					yield return new object[] 
					{ 
						$"{i}. Концерт {i}", 
						$"Описание: Концерт {i}", 
						DateTime.Now.AddDays(i+1), 
						DateTime.Now.AddDays(i + 2) 
					};
				else
					yield return new object[] 
					{ 
						$"{i}. Концерт {i}", 
						$"Описание: Концерт {i}", 
						DateTime.Now.AddDays(-i), 
						DateTime.Now.AddDays(i + 4) 
					};
			}
		}

		IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
	}
}
