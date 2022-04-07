using Extensions;
using UnityEngine;

[CreateAssetMenu(menuName = "Create/Level/Level data", fileName = "Level data")]
public class LevelModel : ScriptableObject
{
    [SerializeField] private float padding;
    internal float Padding => padding;
    
    [SerializeField] private Vector2 lineLengthRange;
    internal Vector2 LineLengthRange => lineLengthRange;
    
    [SerializeField, DoubleRange(0f, 80f)] private Vector2 lineAngleRange;
    internal Vector2 LineAngleRange => lineAngleRange;

    [SerializeField] private float lineWidth = 1f;
    internal float LineWidth => lineWidth;
    [SerializeField] private int _linesCount = 3;
    internal int LinesCount => _linesCount;

    [SerializeField] private Vector2 planeSize;
    internal Vector2 PlaneSize => planeSize;
}