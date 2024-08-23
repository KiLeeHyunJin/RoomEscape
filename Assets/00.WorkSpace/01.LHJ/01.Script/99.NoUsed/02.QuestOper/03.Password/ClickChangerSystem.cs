using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.AddressableAssets;

public class ClickChangerSystem : MonoBehaviour
{
    [SerializeField] public string[] changerArrayNum;
    [SerializeField] string password;
    [SerializeField] AssetReferenceGameObject clicks;
    GameObject removeRef;
    ClickChanger[] childArray;
    StringBuilder builder;
    private void Start()
    {
        builder = new StringBuilder();
        Manager.Resource.GetHandle<GameObject>(clicks, "card", (oper) =>
        {
            removeRef = oper.Result;
            PooledObject pooled = removeRef.GetComponent<ClickChanger>();
            Manager.Pool.CreatePool(pooled, changerArrayNum.Length, changerArrayNum.Length);
            childArray = new ClickChanger[changerArrayNum.Length];
            for (int i = 0; i < changerArrayNum.Length; i++)
            {
                childArray[i] = Manager.Pool.GetPool(pooled).GetComponent<ClickChanger>();
                childArray[i].SetParent(this);
                childArray[i].transform.SetParent(transform);
            }
        });
    }

    public void CheckPassword()
    {
        builder.Clear();
        for (int i = 0; i < childArray.Length; i++)
            builder.Append(childArray[i].Text.text);

        if (string.Equals(builder.ToString(), password))
        {
            Clear();
        }
    }

    void Clear()
    {
        Debug.Log("Á¤´ä!");
        Destroy(gameObject);
    }

    private void OnDestroy()
    {
        Addressables.ReleaseInstance(removeRef);
    }
}
