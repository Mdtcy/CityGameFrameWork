using System.Collections.Generic;
using UnityEngine;

namespace CityBuilderCore
{
    public class ExpandableVisual : MonoBehaviour
    {
        public ExpandableBuildingInfo Info;
        public ExpandableBuilding Building;
        public Transform Pivot;

        public Transform StartPart;
        public Transform RepeatingPart;
        public Transform EndPart;

        private List<Transform> _repeatedParts = new List<Transform>();

        protected virtual void Start()
        {
            if (Building)
            {
                UpdateVisual(Building.Expansion);
                Building.ExpansionChanged += UpdateVisual;
            }

            if (RepeatingPart)
            {
                RepeatingPart.gameObject.SetActive(false);
            }
        }

        public virtual void UpdateVisual(Vector2Int expansion)
        {
            updatePivot(expansion);

            if (Info.IsArea)
                updateAreaObjects(expansion);
            else
                updateLinearObjects(expansion);
        }

        private void updatePivot(Vector2Int expansion)
        {
            if (Pivot)
            {
                var size = Info.Size + expansion + Info.SizePost;

                if (Dependencies.Get<IMap>().IsXY)
                    Pivot.localPosition = new Vector3(size.x / 2f, size.y / 2f, 0f);
                else
                    Pivot.localPosition = new Vector3(size.x / 2f, 0f, size.y / 2f);
            }
        }

        private void instanceParts(int count)
        {
            if (!RepeatingPart)
                return;

            while (_repeatedParts.Count < count)
            {
                _repeatedParts.Add(Instantiate(RepeatingPart, RepeatingPart.parent));
            }

            while (_repeatedParts.Count > count)
            {
                Destroy(_repeatedParts[_repeatedParts.Count - 1].gameObject);
                _repeatedParts.RemoveAt(_repeatedParts.Count - 1);
            }
        }

        private void updateLinearObjects(Vector2Int expansion)
        {
            var map = Dependencies.Get<IMap>();
            var repeats = Mathf.Max(0, expansion.x);

            var cellOffset = map.CellOffset.x;
            var pivotOffset = (Info.Size.x + expansion.x + Info.SizePost.x) * map.CellOffset.x / 2f;

            instanceParts(repeats);

            if (StartPart)
            {
                setLinearPosition(0, StartPart, cellOffset, pivotOffset);
            }

            if (RepeatingPart)
            {
                for (int i = 0; i < repeats; i++)
                {
                    var part = _repeatedParts[i];
                    part.gameObject.SetActive(true);

                    setLinearPosition(Info.Size.x + i, part, cellOffset, pivotOffset);
                }
            }

            if (EndPart)
            {
                setLinearPosition(Info.Size.x + repeats, EndPart, cellOffset, pivotOffset);
            }
        }

        private void setLinearPosition(int x, Transform transform, float cellOffset, float pivotOffset)
        {
            transform.localPosition = new Vector3((x + 0.5f) * cellOffset - pivotOffset, transform.localPosition.y, transform.localPosition.z);
        }

        private void updateAreaObjects(Vector2Int expansion)
        {
            var map = Dependencies.Get<IMap>();
            var repeats = Mathf.Max(0, expansion.x * expansion.y);

            var cellOffset = map.CellOffset.x;
            var pivotOffset = new Vector2((Info.Size.x + expansion.x + Info.SizePost.x) * map.CellOffset.x / 2f, (Info.Size.y + expansion.y + Info.SizePost.y) * map.CellOffset.y / 2f);

            instanceParts(repeats);

            if (StartPart)
            {
                setAreaPosition(Vector2Int.zero, StartPart, cellOffset, pivotOffset, map.IsXY);
            }

            if (RepeatingPart)
            {
                for (int x = 0; x < expansion.x; x++)
                {
                    for (int y = 0; y < expansion.y; y++)
                    {
                        var part = _repeatedParts[x * expansion.y + y];
                        part.gameObject.SetActive(true);

                        setAreaPosition(Info.Size + new Vector2Int(x, y), part, cellOffset, pivotOffset, map.IsXY);
                    }
                }
            }

            if (EndPart)
            {
                setAreaPosition(Info.Size + expansion, EndPart, cellOffset, pivotOffset, map.IsXY);
            }
        }

        private void setAreaPosition(Vector2Int position, Transform transform, float cellOffset, Vector2 pivotOffset, bool isXY)
        {
            if (isXY)
                transform.localPosition = new Vector3((position.x + 0.5f) * cellOffset - pivotOffset.x, (position.y + 0.5f) * cellOffset - pivotOffset.y, transform.localPosition.z);
            else
                transform.localPosition = new Vector3((position.x + 0.5f) * cellOffset - pivotOffset.x, transform.localPosition.y, (position.y + 0.5f) * cellOffset - pivotOffset.y);
        }
    }
}