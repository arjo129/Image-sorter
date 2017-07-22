using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageSorter
{
    public class FilterableCollection<T> : ObservableCollection<T> where T : IComparable
    {
        private List<T> HiddenObjects;
        public FilterableCollection()
        {
            HiddenObjects = new List<T>();
        }
        public void Sort()
        {
            List<T> sorted = this.OrderBy(x => x).ToList();
            for (int i = 0; i < sorted.Count(); i++)
                this.Move(this.IndexOf(sorted[i]), i);
        }
        public void Filter(Func<T,bool> fn)
        {
            List<int> toBeDeleted = new List<int>();
            for(int i = 0; i < this.Count; i++)
            {
                if (!fn(this[i]))
                {
                    HiddenObjects.Add(this[i]);
                    toBeDeleted.Add(i);
                }
            }
            int deleted_count = 0;
            for(int i = 0; i < toBeDeleted.Count; i++)
            {
                this.RemoveItem(toBeDeleted[i]-deleted_count);
                deleted_count++;
            }
        }
        public void Restore()
        {
            foreach(T item in HiddenObjects)
            {
                this.Add(item);
            }
            HiddenObjects.Clear();
        }
    }
}
