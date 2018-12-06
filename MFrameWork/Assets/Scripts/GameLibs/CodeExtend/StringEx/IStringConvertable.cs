using UnityEngine;
using System.Collections;

namespace MoonCommonLib
{
    public interface IStringConvertable<T>
    {
        string ConvertToString();
        T GetValue(string value);

    }
}
