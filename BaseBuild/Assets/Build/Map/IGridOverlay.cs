/**
 * @author BoLuo
 * @email [ tktetb@163.com ]
 * @create date 22:12:48
 * @modify date 22:12:48
 * @desc []
 */

using UnityEngine;

namespace Build.Map
{
    /// <summary>
    /// displays grid lines overlaying the map
    /// </summary>
    public interface IGridOverlay
    {
        void Show();
        void Hide();
    }
}