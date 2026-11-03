using SA;
using SA.Shape.Unity;
using Unity.Collections;
using Unity.Entities;
using UnityEngine;

namespace SA.Demo
{
    [DisallowMultipleComponent]
    public sealed class ShapeWalkControls : MonoBehaviour
    {
        [Range(-1, 1)] public float Legs;
        [Range(-1, 1)] public float LeftLeg;
        [Range(-1, 1)] public float RightArm;
        public bool FreezeAnimation;
        public Camera PreviewCamera;
        public int Motion;
        public string Status = "Loading character...";
        World world;
        EntityQuery query;
        bool hasQuery;
        int appliedMotion = -1;
        GUIStyle title;
        GUIStyle text;
        Vector2 scroll;
        readonly string[] motions = { "Idle", "Walk", "Run" };

        void OnDisable()
        {
            if (hasQuery && world != null && world.IsCreated) query.Dispose();
            hasQuery = false;
            world = null;
        }

        void Update()
        {
            var active = World.DefaultGameObjectInjectionWorld;
            if (active == null || !active.IsCreated) { Status = "Waiting for the animation world..."; return; }
            if (world != active || !hasQuery)
            {
                OnDisable();
                world = active;
                query = world.EntityManager.CreateEntityQuery(typeof(ShapeDriven), typeof(ShapeChannelState), typeof(SharedContribution));
                hasQuery = true;
                appliedMotion = -1;
            }
            var count = query.CalculateEntityCount();
            if (count != 1) { Status = count == 0 ? "Loading character..." : "This view needs one shape character; found " + count + "."; return; }
            var manager = world.EntityManager;
            manager.CompleteAllTrackedJobs();
            using var entities = query.ToEntityArray(Allocator.Temp);
            var entity = entities[0];
            var channels = manager.GetBuffer<ShapeChannelState>(entity);
            ShapeControl.SetRequested(channels, 1, Legs);
            ShapeControl.SetRequested(channels, 2, LeftLeg);
            ShapeControl.SetRequested(channels, 3, RightArm);
            var contributions = manager.GetBuffer<SharedContribution>(entity);
            Motion = Mathf.Clamp(Motion, 0, motions.Length - 1);
            if (Motion != appliedMotion)
            {
                SharedFamilyControl.PlaySingle(ref contributions, Motion);
                appliedMotion = Motion;
            }
            for (var i = 0; i < contributions.Length; i++)
            {
                var value = contributions[i];
                value.Speed = FreezeAnimation ? 0f : 1f;
                contributions[i] = value;
            }
            Status = motions[Motion] + (FreezeAnimation ? " paused" : " playing in place");
        }

        void LateUpdate()
        {
            if (PreviewCamera == null) return;
            var left = Mathf.Min(350f / Mathf.Max(Screen.width, 1), .5f);
            PreviewCamera.rect = new Rect(left, 0, 1f - left, 1);
        }

        public void Neutral()
        {
            Legs = 0; LeftLeg = 0; RightArm = 0;
        }

        void OnGUI()
        {
            if (title == null)
            {
                title = new GUIStyle(GUI.skin.label) { fontSize = 25, fontStyle = FontStyle.Bold, wordWrap = true };
                text = new GUIStyle(GUI.skin.label) { fontSize = 14, wordWrap = true };
            }
            GUILayout.BeginArea(new Rect(18, 20, Mathf.Min(312, Screen.width - 36), Mathf.Max(100, Screen.height - 40)), GUI.skin.box);
            scroll = GUILayout.BeginScrollView(scroll);
            GUILayout.Label("Shape workshop", title);
            GUILayout.Label("Shared clips, adjustable proportions.", text);
            GUILayout.Space(18);
            GUILayout.Label(Status, text);
            GUILayout.Space(16);
            Motion = GUILayout.Toolbar(Motion, motions, GUILayout.Height(32));
            FreezeAnimation = GUILayout.Toggle(FreezeAnimation, "Pause clip · keep shape controls active");
            GUILayout.Space(20);
            Legs = Slider("Both legs", Legs);
            LeftLeg = Slider("Left leg", LeftLeg);
            RightArm = Slider("Right arm", RightArm);
            GUILayout.Space(16);
            if (GUILayout.Button("Restore neutral", GUILayout.Height(34))) Neutral();
            if (GUILayout.Button("Longer legs and arm", GUILayout.Height(30))) { Legs = 1; LeftLeg = 0; RightArm = 1; }
            if (GUILayout.Button("Asymmetric proportions", GUILayout.Height(30))) { Legs = 0; LeftLeg = 1; RightArm = -.5f; }
            GUILayout.Space(18);
            GUILayout.Label("Drag a slider while a clip plays. Switch clips to see the animation transition on the changed body.", text);
            GUILayout.Space(8);
            GUILayout.Label("This view previews skin and joint proportions. Terrain planting and live ragdoll resizing are separate checks.", text);
            GUILayout.EndScrollView();
            GUILayout.EndArea();
        }

        float Slider(string label, float value)
        {
            GUILayout.Label(label + "   " + value.ToString("+0.00;-0.00;0.00"), text);
            value = GUILayout.HorizontalSlider(value, -1f, 1f);
            GUILayout.Space(10);
            return value;
        }
    }
}
