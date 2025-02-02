using System.Globalization;
using TMPro;
using UnityEngine;

namespace Components
{
public struct GateLeaf
{
    public SpriteRenderer LeafBg;
    public TextMeshPro LeafLabel;
    public int Value;
}

//This component stores specific gate data.
    public class Gates : MonoBehaviour
    {
        [SerializeField] private Vector2 _startValues;

        private GateLeaf _leftLeaf;
        private GateLeaf _rightLeaf;

        private bool _isCatalysed;

        public void Initialize(Color positiveColor, Color negativeColor)
        {
            var spriteRenderers = GetComponentsInChildren<SpriteRenderer>();
            var tmPros = GetComponentsInChildren<TextMeshPro>();

            _leftLeaf.LeafBg = spriteRenderers[0];
            _leftLeaf.LeafLabel = tmPros[0];

            _rightLeaf.LeafBg = spriteRenderers[1];
            _rightLeaf.LeafLabel = tmPros[1];

            _leftLeaf.Value = (int) (_startValues.x);
            _rightLeaf.Value = (int) (_startValues.y);

            _leftLeaf.LeafLabel.text = _leftLeaf.Value > 0
                ? "+" + _leftLeaf.Value
                : _leftLeaf.Value.ToString(CultureInfo.InvariantCulture);
            _rightLeaf.LeafLabel.text = _rightLeaf.Value > 0
                ? "+" + _rightLeaf.Value
                : _rightLeaf.Value.ToString(CultureInfo.InvariantCulture);

            _leftLeaf.LeafBg.color = _startValues.x >= 0 ? positiveColor : negativeColor;
            _rightLeaf.LeafBg.color = _startValues.y >= 0 ? positiveColor : negativeColor;
        }

        //This method retrieves the unit-count-change number and then deactivates the gates accordingly.
        public int Catalyse(Vector3 manPos)
        {
            if (_isCatalysed)
                return 0;

            _isCatalysed = true;

            var leftDost = (_leftLeaf.LeafBg.transform.position - manPos).sqrMagnitude;
            var rightDost = (_rightLeaf.LeafBg.transform.position - manPos).sqrMagnitude;

            if (leftDost < rightDost)
            {
                _leftLeaf.LeafBg.enabled = false;
                _leftLeaf.LeafLabel.enabled = false;
                return _leftLeaf.Value;
            }

            _rightLeaf.LeafBg.enabled = false;
            _rightLeaf.LeafLabel.enabled = false;
            return _rightLeaf.Value;
        }
    }
}