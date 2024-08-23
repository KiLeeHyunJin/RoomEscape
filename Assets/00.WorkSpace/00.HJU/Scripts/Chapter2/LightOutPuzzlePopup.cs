using UnityEngine;
using UnityEngine.UI;

public class LightOutPuzzlePopup : PopUpUI
{
    private int gridSize = 3;
    [SerializeField] GameObject[,] grid;
    [SerializeField] GridLayoutGroup cellGroub;
    public GameObject cellPrefab;
    public Color onColor = Color.white;
    public Color offColor = Color.black;
    private bool[,] goalState;
    [SerializeField] GameObject clearMessage;

    protected override void Awake()
    {
        base.Awake();
        InitCell();
    }
    void InitCell()
    {
        RectTransform cellRect = (cellGroub.transform as RectTransform);
        cellRect.anchorMin = new Vector2(cellRect.anchorMin.x, 0.5f);
        cellRect.anchorMax = new Vector2(cellRect.anchorMax.x, 0.5f);
        float width = cellRect.rect.width;
        Debug.Log($"width {width}");
        cellRect.sizeDelta += new Vector2(0, width * 0.5f);

        float cellSize = width * 0.25f;
        float paddingSize = width * 0.125f * 0.5f;
        cellGroub.cellSize = Vector2.one * cellSize;
        cellGroub.spacing = Vector2.one * paddingSize;
    }

    protected override void  Start()
    {
        grid = new GameObject[gridSize, gridSize];
        goalState = new bool[gridSize, gridSize];
        InitCell();
        CreateGrid();
        SetGoalState();
    }

    /// <summary>
    /// 그리드 생성, 셀 초기화
    /// </summary>
    void CreateGrid()
    {
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                GameObject cell = Instantiate(cellPrefab, new Vector3(0, 0, 0), Quaternion.identity);
                cell.transform.SetParent(cellGroub.transform);
                cell.GetComponent<LightOutCell>().Setup(this, x, y);
                grid[x, y] = cell;
            }
        }
    }

    /// <summary>
    /// 해당 셀 상태변경
    /// </summary>
    public void ToggleCell(int x, int y)
    {
        if (x >= 0 && x < gridSize && y >= 0 && y < gridSize)
        {
            GameObject cell = grid[x, y];
            cell.GetComponent<LightOutCell>().Toggle();
        }
    }

    /// <summary>
    /// 선택한 셀을 기준으로 근접한 셀들의 상태변경, 답 체크
    /// </summary>
    public void CellClicked(int x, int y)
    {
        ToggleCell(x, y);
        ToggleCell(x + 1, y);
        ToggleCell(x - 1, y);
        ToggleCell(x, y + 1);
        ToggleCell(x, y - 1);

        CheckForWin();
    }

    /// <summary>
    /// 답 설정
    /// </summary>
    private void SetGoalState()
    {
        goalState[0, 0] = true;
        goalState[0, 1] = true;
        goalState[0, 2] = true;
        goalState[1, 0] = false;
        goalState[1, 1] = false;
        goalState[1, 2] = true;
        goalState[2, 0] = false;
        goalState[2, 1] = false;
        goalState[2, 2] = true;
    }

    /// <summary>
    /// 현재 상태가 설정한 답과 일치하는지 확인
    /// </summary>
    private void CheckForWin()
    {
        bool isWin = true;
        for (int x = 0; x < gridSize; x++)
        {
            for (int y = 0; y < gridSize; y++)
            {
                //그리드의 셀을 순환하면서 답과 일치하지 않은 셀이 있으면 false
                if (grid[x, y].GetComponent<LightOutCell>().IsOn() != goalState[x, y])
                {
                    isWin = false;
                    break;
                }
            }
            if (!isWin)
            {
                break;
            }
        }

        if (isWin)
        {
            Manager.Chapter._clickObject.state = 1;
            clearMessage.SetActive(true);
            Manager.Chapter.HintData.SetClearQuestion(6);
            Debug.Log("You Win!");
        }
    }
}


