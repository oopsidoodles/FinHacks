using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseScraper
{
    static class RonysSortingAlgorithm //Written by Vili
    {
        public static MenuItem[] SortByPrice(MenuItem[] items, int start, int end)
        {
            if (start == end)
            {
                if (items[start] == null)
                {

                }
                return new MenuItem[1] { items[start] };
            }

            int mp = (int)((start + end) / 2f);
            MenuItem[] array1 = SortByPrice(items, start, mp);
            MenuItem[] array2 = SortByPrice(items, mp + 1, end);
            int index1 = 0;
            int index2 = 0;
            int index = 0;
            MenuItem[] result = new MenuItem[array1.Length + array2.Length];
            for (int a = 0; a < (array1.Length + array2.Length); a++)
            {
                // array1 is depleted
                if (index1 >= array1.Length)
                {
                    result[index] = array2[index2];
                    index2++;
                }
                // array2 is depleted
                else if (index2 >= array2.Length)
                {
                    result[index] = array1[index1];
                    index1++;
                }
                // Both have elements
                else
                {
                    MenuItem item1 = array1[index1];
                    MenuItem item2 = array2[index2];
                    float val1 = item1.Price;
                    float val2 = item2.Price;

                    // array1 element before array2 element
                    if (val1 < val2)
                    {
                        result[index] = array1[index1];
                        index1++;
                    }
                    // array2 element before array1 element
                    else if (val2 < val1)
                    {
                        result[index] = array2[index2];
                        index2++;
                    }
                    else
                    {
                        Random rand = new Random();
                        int choice = rand.Next(0, 2);
                        if (choice == 0)
                        {
                            result[index] = array1[index1];
                            index1++;
                        }
                        else if (choice == 2)
                        {
                            result[index] = array2[index2];
                            index2++;
                        }
                    }
                }
                index++;
            }
            return result;
        }
    }
}
