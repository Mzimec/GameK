using UnityEngine;
using UnityEngine.UIElements;

[UxmlElement]
public partial class TestUIT : VisualElement {
    public TestUIT() {
        Add(new Label());
    }
}
