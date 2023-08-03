using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SLightParameterManager {
    public static class Utils {

        public static List<Control> AllControls(this Control control) {
            var unvisited = new Queue<Control>();
            var res = new Queue<Control>();
            unvisited.Enqueue(control);
            while(unvisited.Count > 0) {
                var next = unvisited.Dequeue();
                foreach(Control child in next.Controls) {
                    unvisited.Enqueue(child);
                }
                res.Enqueue(next);
            }
            return res.ToList();
        }

        public static IEnumerable<R> OfClass<T, R>(this IEnumerable<T> enumerable) where R:class {
            return enumerable.Where(control => control is R).Select(control => control as R);
        }

        public static IEnumerable<T> NotNull<T>(this IEnumerable<T> enumerable) {
            return enumerable.Where(tipo => tipo != null);
        }
    }
}
