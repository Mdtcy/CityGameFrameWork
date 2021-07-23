/**
 * @author BoLuo
 * @email [ tktetb@163.com ]
 * @create date 22:37:32
 * @modify date 22:37:32
 * @desc []
 */

using System;
using UnityEngine;

namespace Build.Structure
{
    [Serializable]
    public class StructureLevelMask
    {
        public int Value;

        public bool Check(int value) => Check(Value, value);

        /// <summary>
        /// 转化成二进制的1位有重合,也就是又被Mask到
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <returns></returns>
        public static bool Check(int a, int b) => a == 0 || b == 0 || (a & b) != 0;
    }
}