using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Roacher : MonoBehaviour
{
    public List<Sprite> RoachSprites;
    public Transform Player;
    public Roach Roach;

    public List<Transform> Points;
    List<Transform> RevPoints;

    public int KillCount = 0;
    public int SpawnCount = 0;

    int idx; // inside Points
    bool _ab; // or b->a
    bool _alive;
    float _lerpDur;
    float _startTime;
    bool _force = true;

    private void Update()
    {
        if (!_alive) return;

        var since = Time.time - _startTime;
        var pct = since / _lerpDur;
        Roach.transform.position = Vector3.Lerp(
            _ab ? Points[idx].position : RevPoints[idx].position,
            _ab ? Points[idx+1].position : RevPoints[idx+1].position,
            Easing.Quartic.Out(pct));

        if(pct >= 1.0f)
        {
            if (idx >= (Points.Count - 2))
            {
                _alive = false;
                Roach.gameObject.SetActive(false);
            }
            else
            {
                _startTime = Time.time;
                idx++;
            }
        }
    }

    private void Start()
    {
        RevPoints = Points.ToList();
        RevPoints.Reverse();

        Roach.OnStep = OnStep;
    }

    public void MaybeSpawn()
    {
        var r = UnityEngine.Random.value;
        Debug.Log($"maybe spawn={r}");
        if (r >= 0.5f || _force)
        {
            Spawn();
            _force = false;
        }
    }

    void Spawn()
    {
        var rand = new System.Random();
        var r = rand.Next();
        _ab = r % 2 == 0;
        _lerpDur = 1f;
        _startTime = Time.time;
        _alive = true;
        idx = 0;
        Roach.gameObject.SetActive(true);
        Roach.Renderer.flipX = _ab;
        SpawnCount++;
    }

    Vector3 _prevPlayerPos;
    void OnStep()
    {
        _alive = false;
        KillCount++;
        AudioManager.ins.Play(AudioManager.ins.SquishRoach);
        _prevPlayerPos = Player.position;
        var newPos = Roach.transform.position;
        newPos.z = _prevPlayerPos.z;
        Player.position = newPos;
        Roach.gameObject.SetActive(false);
        StartCoroutine(ReturnPlayer());
    }

    IEnumerator ReturnPlayer()
    {
        yield return new WaitForSeconds(1f);
        Player.position = _prevPlayerPos;
    }
}
