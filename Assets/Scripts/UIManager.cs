using UnityEngine;
using Mirror;
using TMPro;
using System.Collections;

public class UIManager : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI hostScoreText;
    [SerializeField] private TextMeshProUGUI clientScoreText;
    [SerializeField] private TextMeshProUGUI winText;
    [SerializeField] private TextMeshProUGUI loseText;
    [SerializeField] private GameObject pausePopUp;
    [SerializeField] private GameObject startingText;
    [SerializeField] private GameObject countDown;

    [SyncVar(hook = nameof(UpdateHostScore))] private int _hostScore = 0;
    [SyncVar(hook = nameof(UpdateClientScore))] private int _clientScore = 0;

    private void OnEnable()
    {
        Messenger.AddListener(GameEvent.GAME_STARTED, RpcOnGameStarted);
        Messenger<string>.AddListener(GameEvent.SCORE_UPDATED, OnScoreUpdated);
    }

    private void OnDisable()
    {
        Messenger.RemoveListener(GameEvent.GAME_STARTED, RpcOnGameStarted);
        Messenger<string>.RemoveListener(GameEvent.SCORE_UPDATED, OnScoreUpdated);
    }

    private void Start()
    {
        if (isClientOnly)
        {
            (hostScoreText.transform.position, clientScoreText.transform.position) = (clientScoreText.transform.position, hostScoreText.transform.position);
        }
    }

    [ClientRpc]
    private void RpcOnGameStarted()
    {
        startingText.SetActive(false);

        StartCoroutine(CountDown());
    }

    private IEnumerator CountDown()
    {
        countDown.SetActive(true);

        yield return new WaitForSeconds(1f);

        for (int i = 1; i < 4; i++)
        {
            countDown.GetComponent<TextMeshProUGUI>().text = i.ToString();

            yield return new WaitForSeconds(1f);
        }

        countDown.SetActive(false);
    }

    private void OnScoreUpdated(string tag)
    {
        if (tag == "HostGoal")
        {
            _clientScore++;
        }
        else if (tag == "ClientGoal")
        {
            _hostScore++;
        }

        if (_hostScore == 7)
        {
            winText.gameObject.SetActive(true);
            RpcShowEndingText(false);

            Messenger.Broadcast(GameEvent.PLAYER_WON);
        }
        else if (_clientScore == 7)
        {
            loseText.gameObject.SetActive(true);
            RpcShowEndingText(true);

            Messenger.Broadcast(GameEvent.PLAYER_WON);
        }
    }

    [ClientRpc]
    private void RpcShowEndingText(bool isWin)
    {
        if (isClientOnly)
        {
            if (isWin)
            {
                winText.gameObject.SetActive(true);
            }
            else
            {
                loseText.gameObject.SetActive(true);
            }
        }
    }

    private void UpdateHostScore(int oldScore, int newScore)
    {
        hostScoreText.text = newScore.ToString();
    }

    private void UpdateClientScore(int oldScore, int newScore)
    {
        clientScoreText.text = newScore.ToString();
    }

    public void Pause()
    {
        pausePopUp.SetActive(true);
    }

    public void Resume()
    {
        pausePopUp.SetActive(false);
    }

    public void Exit()
    {
        Application.Quit();
    }
}
