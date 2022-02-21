using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PlayerCollider : MonoBehaviour
{
    public static event Action OnPickupCoin;
    public static event Action OnPickupDiamond;
    public static event Action<NeuralNetwork> OnPlayerDeadNN;
    public static event Action<BackPropNetwork> OnPlayerDeadNNBP;
    public static event Action<string> OnPowerUp;
    bool isBackProp = false;
    public void setNN(bool isbackprop)
    {
        isBackProp = isbackprop;
    }
    private void OnTriggerEnter2D(Collider2D collision)
    {
        switch (collision.gameObject.tag)
        {
            case "Coin":
                {
                    OnPickupCoin?.Invoke();
                    Destroy(collision.gameObject);
                    break;
                }
            case "Diamond":
                {
                    OnPickupDiamond?.Invoke();
                    Destroy(collision.gameObject);
                    break;
                }
            case "Enemy":
                {
                    if(!isBackProp)
                    {
                        OnPlayerDeadNN?.Invoke(this.GetComponent<NeuralNetworkComponent>().GetNetwork());
                    }
                    else
                    {
                        OnPlayerDeadNNBP?.Invoke(this.GetComponent<BackPropNetworkComponent>().GetNetwork());
                    }
                    Destroy(gameObject);
                    break;
                }
            case "PowerUp":
                {
                    OnPowerUp?.Invoke(collision.gameObject.name);
                    Destroy(collision.gameObject);
                    break;
                }
        }
    }
}
