using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace CityBuilderCore
{
    public class WorkersPanel : MonoBehaviour
    {
        public Image[] Icons;

        public void SetWorkers(IEnumerable<Worker> workers)
        {
            int count = workers.Count();
            for (int i = 0; i < Icons.Length; i++)
            {
                if (i >= count)
                {
                    Icons[i].gameObject.SetActive(false);
                }
                else
                {
                    Icons[i].gameObject.SetActive(true);

                    var worker = workers.ElementAt(i);
                    if (worker == null)
                    {
                        Icons[i].sprite = null;
                        Icons[i].color = Color.clear;
                    }
                    else
                    {
                        Icons[i].sprite = worker.Icon;
                        Icons[i].color = Color.white;
                    }
                }
            }
        }
    }
}
