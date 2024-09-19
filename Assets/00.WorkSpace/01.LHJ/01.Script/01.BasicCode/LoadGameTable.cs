using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadGameTable : MonoBehaviour
{
    [SerializeField] BackGroundSizeSetting field;
    [SerializeField] RectTransform safeArea;
    [SerializeField] bool loadField;
    public GameObject InventoryPanel { get { return inventoryPanel; } }
    GameObject inventoryPanel;
    InventoryButton inventoryButton;

    ChapterSaveLoad saveLoad;

    private void Start()
    {
        //필드의 사이즈 설정 컴포넌트가 없을 경우 가져온다.
        if (field == null)
            field = GetComponentInChildren<BackGroundSizeSetting>();

        //세이브 로드 컴포넌트가 없을 경우
        if (saveLoad == null)
        {
            if (field != null)
            {
                //필드 오브젝트에서 세이브 가져온다.
                //필드 오브젝트에 챕터 세이브 로드 컴포넌트가 없을 시 부착한다.
                saveLoad = field.gameObject.GetOrAddComponent<ChapterSaveLoad>();
            }
        }

        //세이프영역 컴포넌트가 없을 경우
        if (safeArea == null)
        {
            //영역설정 컴포넌트를 가져와서 거기서 세이프영역을 가져온다.
            safeArea = GetComponent<ResolutionChangeScaler>().SafeArea;
            //역역 설정 컴포넌트가 없을 경우
            if (safeArea == null)
            {
                //객체중 세이프영역이름의 자식을 찾는다.
                foreach (Transform child in transform)
                {
                    if ("safeArea" == child.gameObject.name)
                    {
                        safeArea = child as RectTransform;
                        break;
                    }
                }
            }
        }
        //로드필드가 참일 경우 
        if (loadField)
        {
            //번들에서 필드를 로드한다.
            CreateChapterField($"chapter0{Manager.Chapter.chapter}", "Field");
        }
        else
        {
            //참이 아닐경우 이어하기만 실행한다.
            LoadChapterData();
        }

        //인벤토리 패널을 로드한다.
        CreateInventoryPanel();
        //인게임 ui를 로드한다.
        CreatInGameUI();

    }

    /// <summary>
    /// 인게임 UI를 가져온다.
    /// </summary>
    void CreatInGameUI()
    {
        //인게임 UI로드를 시도한다.
        Manager.Resource.GetAsset
            (bundleName : "basic", fileName : "PlayInGameUI", 
             ResourceType.GameObject, (obj) =>
        {
            //로드한 객체 데이터를 복제한다.
            GameObject ingameUI = Instantiate((GameObject)obj);
            //해당 복제 오브젝트의 하위의 텍스트 컴포넌트가 있는지 확인하고 있을경우 폰트를 입힌다.
            ingameUI.FontInit(Define.Font.MLight);
            //설정 영역이 존재하면 해당 하위에 인게임 UI를 배치한다.
            if (safeArea != null)
                ingameUI.transform.SetParent(safeArea.transform);
            //인게임 UI 객체의 크기 및 위치값 초기화를 시도한다.
            TransformInit(ingameUI);

        }, false);
    }
    /// <summary>
    /// 인벤토리 UI를 가져온다.
    /// </summary>
    void CreateInventoryPanel()
    {
        //인벤토리 UI의 로드를 시도한다.
        Manager.Resource.GetAsset("basic", "Inventory", ResourceType.GameObject, (obj) =>
        {
            //로드한 객체 데이터를 복제한다.
            GameObject inven = Instantiate((GameObject)obj);
            //해당 복제 오브젝트의 하위의 텍스트 컴포넌트가 있는지 확인하고 있을경우 폰트를 입힌다.
            inven.FontInit(Define.Font.MLight);

            //현재 오브젝트 하위에 인벤토리 UI를 배치한다.
            inven.transform.SetParent(this.transform);
            //인게임 UI 객체의 크기 및 위치값 초기화를 시도한다.
            TransformInit(inven);
            //인벤토리 패널을 복제 오브젝트로 연결한다.
            inventoryPanel = inven;
            //하위 객체에서 인벤토리 버튼을 가져와서 연결한다.
            inventoryButton = GetComponentInChildren<InventoryButton>();
            //가져온 인벤토리 버튼이 Null이 아니라면 
            if (inventoryButton != null)
            {
                //해당 인벤토리 버튼에 현재 복제한 인벤토리를 연결한다.
                inventoryButton.SetInventoryPanel(inven);
            }
        }, false);
    }

    /// <summary>
    /// 챕터 필드의 로드를 시도한다.
    /// </summary>
    void CreateChapterField(string chapter, string objName)
    {
        //챕터 필드의 로드를 시도한다.
        Manager.Resource.GetAsset(chapter, objName, ResourceType.GameObject, (obj) =>
        {
            //필드 영역을 배치할 위치가 있는지 확인한다.
            if (field == null)
                return;
            //오브젝트를 형변환한다.
            GameObject fieldObj = (GameObject)obj;
            if (fieldObj == null)
                return;
            //로드한 객체 데이터를 복제한다.
            fieldObj = Instantiate((GameObject)obj);
            //복제 오브젝트의 하위의 텍스트 컴포넌트가 있는지 확인하고 있을경우 폰트를 입힌다.
            fieldObj.FontInit(Define.Font.MLight);
            //현재 오브젝트 하위에 인벤토리 UI를 배치한다.
            fieldObj.transform.SetParent(field.transform);
            //필드 영역의 하위 객체 크기 재설정을 시도한다.
            field.InitSize();
            //해당 챕터의 데이터 정보를 가져온다.
            LoadChapterData();
        }, false);
    }

    void LoadChapterData()
    {
        //세이브 데이터 로드를 시도한다면
        if (Manager.Chapter.ContinueState)
        {
            //세이브 로드를 시도할 챕터의 번호를 가져온다.
            saveLoad.LoadChapterNum();
            //해당 챕터의 기믹의 클리어 유무 배열을 가져온다.
            saveLoad.LoadCurrentChpater(ContinueLoadChapter);
        }
        else
        {
            ContinueLoadChapter(null);
            //saveLoad.LoadCurrentChpater(null); //<- 이렇게 하면 힌트 오류남
        }
        //힌트 데이터 로드를 시도한다.
       
        //세이브 데이터를 삭제한다.
        //saveLoad.RemoveSaveFile(Manager.Chapter.chapter);
    }




    void ContinueLoadChapter(List<bool> clearStateList)
    {
        Manager.Chapter.LoadHintDataBase(clearStateList, Manager.Chapter.chapter);
    }

    /// <summary>
    /// 오브젝트의 위치값 및 크기를 재설정한다.
    /// </summary>
    void TransformInit(GameObject obj)
    {
        //다운캐스팅을 한다.
        RectTransform rect = obj.transform as RectTransform;
        //비율을 1배수로 설정한다.
        rect.localScale = Vector2.one;
        //앵커의 위치를 중앙으로 설정한다.
        rect.anchoredPosition = Vector2.zero;
        //오프셋 범위를 0으로 설정한다.
        rect.offsetMin = rect.offsetMax = Vector2.zero;
    }

}
