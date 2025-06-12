using UnityEngine;
using Mirror;
using System.Collections;

public class NetworkManagerGH : NetworkManager
{
    private GameObject[] playerSpawns;
    private GameObject[] ballSpawns;
    private GameObject _ball;
    private bool _isFirstTime;

    public void OnEnable()
    {
        Messenger<string>.AddListener(GameEvent.SCORE_UPDATED, OnScoreUpdated);
        Messenger.AddListener(GameEvent.PLAYER_WON, OnPlayerWon);
    }

    public void OnDisable()
    {
        Messenger<string>.RemoveListener(GameEvent.SCORE_UPDATED, OnScoreUpdated);
        Messenger.RemoveListener(GameEvent.PLAYER_WON, OnPlayerWon);
    }

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        playerSpawns = GameObject.FindGameObjectsWithTag("Player Spawn");
        ballSpawns = GameObject.FindGameObjectsWithTag("Ball Spawn");

        GameObject playerSpawn = conn.connectionId == 0 ? playerSpawns[0] : playerSpawns[1];
        GameObject player = Instantiate(playerPrefab, playerSpawn.transform.position, playerSpawn.transform.rotation);
        NetworkServer.AddPlayerForConnection(conn, player);

        if (numPlayers == 2)
        {
            _isFirstTime = true;

            StartCoroutine(SpawnBall(ballSpawns[0].transform));

            Messenger.Broadcast(GameEvent.GAME_STARTED);
        }
    }

    private IEnumerator SpawnBall(Transform ballSpawn)
    {
        if (_isFirstTime)
        {
            yield return new WaitForSeconds(2f);

            _isFirstTime = false;
        }

        yield return new WaitForSeconds(3f);

        _ball = Instantiate(spawnPrefabs.Find(prefab => prefab.name == "Ball"), ballSpawn.position, ballSpawn.rotation);
        NetworkServer.Spawn(_ball);
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        base.OnServerDisconnect(conn);

        NetworkServer.Destroy(_ball);
    }

    private void OnScoreUpdated(string tag)
    {
        GameObject ballSpawn = new GameObject();

        NetworkServer.Destroy(_ball);

        if (tag == "HostGoal")
        {
            ballSpawn = ballSpawns[1];
        }
        else if (tag == "ClientGoal")
        {
            ballSpawn = ballSpawns[2];
        }

        StartCoroutine(SpawnBall(ballSpawn.transform));
    }

    private void OnPlayerWon()
    {
        StartCoroutine(RestartLevel());
    }

    private IEnumerator RestartLevel()
    {
        yield return new WaitForSeconds(3f);

        ServerChangeScene("Gameplay");
    }
}
