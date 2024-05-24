using UnityEngine;
using Unity.Services.Core;
using Unity.Services.Authentication;
using System.Threading.Tasks;

public class ServicesInitializer : MonoBehaviour
{
    async void Start()
    {
        await UnityServices.InitializeAsync();

        AuthenticationService.Instance.SignedIn += () =>
        {
            Debug.Log("Signed in" + AuthenticationService.Instance.PlayerId);
        };

        await SignInAnonymouslyAsync();
    }

    private async Task SignInAnonymouslyAsync()
    {
        await AuthenticationService.Instance.SignInAnonymouslyAsync();
        //Debug.Log("Signed in anonymously");
    }
}
