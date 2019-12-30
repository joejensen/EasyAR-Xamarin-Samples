using Urho;

namespace EasyARX
{
    /// <summary>
    /// Component which can be added to an UrhoNode in order to render the background
    /// </summary>
    class EasyARBackgroundNode : Component
    {
        private Material Material;

        public EasyARBackgroundNode(Material material)
        {
            this.Material = material;
        }

        public override void OnAttachedToNode(Node node)
        {
            CustomGeometry geometry = node.CreateComponent<CustomGeometry>();
            geometry.BeginGeometry(0, PrimitiveType.TriangleFan);
            geometry.DefineVertex(new Vector3(-1, -1, 0));
            geometry.DefineTexCoord(new Vector2(0.0f, 0.0f));
            geometry.DefineVertex(new Vector3(1, -1, 0));
            geometry.DefineTexCoord(new Vector2(1.0f, 0.0f));
            geometry.DefineVertex(new Vector3(1, 1, 0));
            geometry.DefineTexCoord(new Vector2(1.0f, 1.0f));
            geometry.DefineVertex(new Vector3(-1, 1, 0));
            geometry.DefineTexCoord(new Vector2(0.0f, 1.0f));
            geometry.Commit();
            geometry.SetMaterial(Material);
            geometry.CastShadows = false;

            node.Translate(new Vector3(0, 0, 1));
            base.OnAttachedToNode(node);
        }
    }
}
