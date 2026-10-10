using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public static class CitySceneBuilder
{
    private const string ScenePath = "Assets/Scenes/SampleScene.unity";

    static CitySceneBuilder()
    {
        EditorApplication.delayCall += AutoBuildIfMissing;
    }

    private static void AutoBuildIfMissing()
    {
        if (EditorApplication.isPlayingOrWillChangePlaymode) return;
        var existingCity = GameObject.Find("City Squid Game");
        if (existingCity == null || existingCity.transform.Find("Seoul Asphalt Avenue") == null)
        {
            BuildCityStreetScene();
        }
    }

    [MenuItem("Squid Game/Build Pre-Island City Scene")]
    public static void BuildCityStreetScene()
    {
        var scene = EditorSceneManager.OpenScene(ScenePath);

        // Clean up legacy objects from previous builds
        var rootObjects = scene.GetRootGameObjects();
        foreach (var obj in rootObjects)
        {
            if (obj == null) continue;
            string objName = obj.name;
            if (objName == "City Squid Game" ||
                objName.StartsWith("Main asphalt") ||
                objName.StartsWith("Apartment") ||
                objName.StartsWith("Crosswalk") ||
                objName.StartsWith("Car ") ||
                objName.StartsWith("Tree ") ||
                objName.StartsWith("Street") ||
                objName.StartsWith("Finish ") ||
                objName.StartsWith("Right sidewalk") ||
                objName.StartsWith("Left sidewalk") ||
                objName.StartsWith("Center lane") ||
                objName.StartsWith("Shop") ||
                objName.StartsWith("Parked") ||
                objName.StartsWith("Island transfer"))
            {
                Object.DestroyImmediate(obj);
            }
        }

        var city = new GameObject("City Squid Game");
        var audio = city.AddComponent<AudioSource>();
        audio.playOnAwake = false;
        var controller = city.AddComponent<CityGameController>();
        controller.BuildFullCity();

        EditorSceneManager.MarkSceneDirty(scene);
        EditorSceneManager.SaveScene(scene);
        Debug.Log("[Squid Game] Realistic Seoul City & Salesman Ddakji recruitment scene built successfully!");
    }
}
