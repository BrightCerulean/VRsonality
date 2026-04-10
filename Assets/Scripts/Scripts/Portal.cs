using UnityEngine;
using System.Collections;

public abstract class Portal : MonoBehaviour
{
    bool isTransitioning = false;
    public void Activate(GameObject player, PortalTrigger trigger)
    {
        if (isTransitioning) return;
        StartCoroutine(TeleportSequence(player, trigger));
    }
    private IEnumerator TeleportSequence(GameObject player, PortalTrigger trigger)
    {
        isTransitioning = true;
        // Fade to black
        //yield return fader.StartCoroutine(fader.FadeToBlack());
        yield return ScreenFader.Instance.FadeToBlack();

        // Teleport
        Debug.Log("TELEPORTING");
        PerformTeleport(player);
        yield return null;


        // Fade back in
        //yield return fader.StartCoroutine(fader.FadeFromBlack());
        yield return ScreenFader.Instance.FadeFromBlack();
        isTransitioning = false;
        trigger.ResetTeleport();
        Debug.Log("TELEPORTING DONE");
        Debug.Log("I AM NOW HERE: " + player.transform.position);
    }
    protected abstract void PerformTeleport(GameObject player);
}