using GameplayIngredients;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Experimental.VFX;
using System.Text;

[ManagerDefaultPrefab("VFXDebugManager")]
public class VFXDebugManager : Manager
{
    [Header("UI")]
    public GameObject uiRoot;
    public Text debugText;

    const KeyCode Toggle = KeyCode.F7;
    const KeyCode PrevFX = KeyCode.PageUp;
    const KeyCode NextFX = KeyCode.PageDown;
    const KeyCode Play = KeyCode.I;
    const KeyCode Stop = KeyCode.U;
    const KeyCode Pause = KeyCode.P;
    const KeyCode Reinit = KeyCode.J;
    const KeyCode Step = KeyCode.K;
    const KeyCode Sort = KeyCode.M;
    const KeyCode ToggleVisibility = KeyCode.L;

    bool visible = false;

    private void Update()
    {
        if (Input.GetKeyDown(Toggle))
        {
            visible = !visible;
            uiRoot.SetActive(visible && uiRoot != null);
        }

        if(visible && debugText != null)
        {
            debugText.text = UpdateVFXDebug();
        }
    }

    int selectedVFX = -1;
    Sorting sorting = Sorting.None;

    enum Sorting
    {
        None = 0,
        DistanceToCamera = 1,
        ParticleCount = 2
    }

    string UpdateVFXDebug()
    {
        VisualEffect[] allEffects = VFXManager.GetComponents();

        if (Input.GetKeyDown(Sort))
            sorting = (Sorting)(((int)sorting + 1) % 3);

        if(sorting == Sorting.DistanceToCamera)
        {
            var camera = Camera.main;
            if (camera == null)
            {
                sorting = Sorting.ParticleCount;
            }
            else
            {
                allEffects = allEffects.OrderBy(o => Vector3.SqrMagnitude(o.gameObject.transform.position - camera.transform.position)).ToArray();
            }
        }

        if(sorting == Sorting.ParticleCount)
        {
            allEffects = allEffects.OrderBy(o => -o.aliveParticleCount).ToArray();
        }


        if (allEffects.Length == 0)
            return "No Active VFX Components in scene";

        selectedVFX -= Input.GetKeyDown(PrevFX) ? 1 : 0;
        selectedVFX += Input.GetKeyDown(NextFX) ? 1 : 0;

        bool shift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);

        selectedVFX = Mathf.Clamp(selectedVFX,0, allEffects.Length - 1);

        StringBuilder sb = new StringBuilder();

        sb.AppendLine($"{allEffects.Length} Visual Effect Component(s) active. Sorting : {sorting.ToString()}");
        sb.AppendLine();

        sb.AppendLine($"{"Game Object Name",-24}| {"Visual Effect Asset",-24}| {"PlayState",-12}| {"Visibility",-12}| {"Particle Count",12}");
        sb.AppendLine($"===================================================================================================================================");

        int idx = 0;

        foreach(var vfx in allEffects)
        {
            if (idx == selectedVFX)
                sb.Append("<color=orange>");

            string gameObjectname = vfx.gameObject.name;
            string vfxName = (vfx.visualEffectAsset == null ? "(No VFX Asset)" : vfx.visualEffectAsset.name);
            string playState = (vfx.pause ? "Paused" : "Playing");
            var renderer = vfx.GetComponent<Renderer>();
            string visibility = renderer.enabled ? (vfx.culled? "Culled" : "Visible") : "Disabled";
            string particleCount = vfx.aliveParticleCount.ToString();

            sb.Append($"{gameObjectname, -24}| {vfxName, -24}| {playState,-12}| {visibility,-12}| {particleCount, 12}");

            if (idx == selectedVFX)
                sb.Append("</color>");
            sb.Append("\n");
            idx++;
        }

        var selected = allEffects[selectedVFX];

        // Make Selection Blink
        if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
        {
            var selectedRenderer = selected.GetComponent<Renderer>();
            selectedRenderer.enabled = Time.unscaledTime % 0.5f < 0.25f;
        }

        if (shift)
        {
            foreach (var vfx in allEffects)
            {
                if (Input.GetKeyDown(Play))
                    vfx.Play();
                if (Input.GetKeyDown(Stop))
                    vfx.Stop();
                if (Input.GetKeyDown(Pause))
                    vfx.pause = !vfx.pause;
                if (Input.GetKeyDown(Reinit))
                    vfx.Reinit();
                if (Input.GetKeyDown(Step))
                    vfx.AdvanceOneFrame();
                if (Input.GetKeyDown(ToggleVisibility))
                    vfx.gameObject.GetComponent<Renderer>().enabled = !vfx.gameObject.GetComponent<Renderer>().enabled;
            }
        }
        else
        {
            if (Input.GetKeyDown(Play))
                selected.Play();
            if (Input.GetKeyDown(Stop))
                selected.Stop();
            if (Input.GetKeyDown(Pause))
                selected.pause = !selected.pause ;
            if (Input.GetKeyDown(Reinit))
                selected.Reinit();
            if (Input.GetKeyDown(Step))
                selected.Simulate(Time.deltaTime);
            if (Input.GetKeyDown(ToggleVisibility))
                selected.gameObject.GetComponent<Renderer>().enabled = !selected.gameObject.GetComponent<Renderer>().enabled;
        }

        return sb.ToString();
    }

}
