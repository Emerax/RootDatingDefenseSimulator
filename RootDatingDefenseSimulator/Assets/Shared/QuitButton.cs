using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
[RequireComponent(typeof(Button))]
public class QuitButton : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler {

    [SerializeField]
    private float minAlpha;
    [SerializeField]
    private float maxAlpha;
    [SerializeField]
    private float alphaDeltaPerSecond;

    private Image image;
    private Button button;

    private bool shouldAutoHide = false;
    private float currentAlpha;
    private float targetAlphaAutoHide;

    private void Awake() {
        image = GetComponent<Image>();
        button = GetComponent<Button>();
        button.onClick.AddListener(QuitGame);

        targetAlphaAutoHide = minAlpha;
        currentAlpha = maxAlpha;
        SetImageAlpha(currentAlpha);

        GameLogic.GameStarted += OnGameStarted;
        GameLogic.GameOver += OnGameOver;
    }

    public void OnPointerEnter(PointerEventData eventData) {
        targetAlphaAutoHide = maxAlpha;
    }

    public void OnPointerExit(PointerEventData eventData) {
        targetAlphaAutoHide = minAlpha;
    }

    private void OnGameStarted() {
        shouldAutoHide = true;
    }

    private void OnGameOver() {
        shouldAutoHide = false;
    }

    private void Update() {
        if(shouldAutoHide) {
            if(targetAlphaAutoHide < currentAlpha) {
                currentAlpha -= alphaDeltaPerSecond * Time.deltaTime;
            }
            else if(targetAlphaAutoHide > currentAlpha) {
                currentAlpha += alphaDeltaPerSecond * Time.deltaTime;
            }
        }
        else {
            currentAlpha += alphaDeltaPerSecond * Time.deltaTime;
        }
        currentAlpha = Mathf.Clamp(currentAlpha, minAlpha, maxAlpha);
        SetImageAlpha(currentAlpha);
    }

    private void SetImageAlpha(float alpha) {
        Color color = image.color;
        color.a = alpha;
        image.color = color;
    }

    private void QuitGame() {
        Application.Quit();
    }
}
