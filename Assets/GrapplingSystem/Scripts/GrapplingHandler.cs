using System;
using UnityEngine;
using UnityEngine.Events;
using Alchemy.Inspector;

/// <summary>
/// グラップリングフック機能を管理するクラス
/// SpringJointを使用してプレイヤーと固定ポイント間にバネ式ロープ接続を作成し、
/// 振り子運動やケーブル操作による移動をサポートします。
/// </summary>
public class GrapplingHandler : MonoBehaviour, IGrappleDrawer
{
    /// <summary>グラップルの影響を受けるRigidbody</summary>
    [SerializeField] Rigidbody _rigidbody;
    /// <summary>グラップルケーブルの発射起点Transform</summary>
    [SerializeField] Transform origin;
    /// <summary>グラップル接続先のTransform</summary>
    [SerializeField] Transform destination;

    [Header("parameter")]
    /// <summary>SpringJointの最小距離係数（0-1の範囲）</summary>
    [SerializeField] private float minDistance = 0.25f;
    /// <summary>SpringJointの最大距離係数（0-1の範囲）</summary>
    [SerializeField] private float maxDistance = 0.8f;
    /// <summary>バネの強度（高いほど強く引っ張る）</summary>
    [SerializeField] private float spring = 4.5f;
    /// <summary>バネの減衰率（高いほど振動が早く止まる）</summary>
    [SerializeField] private float damper = 7f;
    /// <summary>質量スケール（接続の強度に影響）</summary>
    [SerializeField] private float massScale = 4.5f;
    /// <summary>前方推進力</summary>
    [SerializeField] private float forwardThrustForce = 200f;
    /// <summary>水平推進力</summary>
    [SerializeField] private float horizontalThrustForce = 200f;
    /// <summary>ケーブル延長速度</summary>
    [SerializeField] private float extendCableSpeed = 10f;

    /// <summary>グラップル中のSpringJointコンポーネント</summary>
    private SpringJoint joint;
    /// <summary>グラップル接続ポイントの世界座標</summary>
    private Vector3 grapplePoint;

    /// <summary>
    /// エディタでパラメータが変更された時に呼ばれる
    /// </summary>
    void OnValidate()
    {
        UpdateJointParameter();
    }

    /// <summary>
    /// 既存のSpringJointのパラメータを更新する
    /// エディタでの値変更を実行時に反映するために使用
    /// </summary>
    private void UpdateJointParameter()
    {
        if (joint == null) return;

        // バネの物理パラメータを現在の設定値で更新
        joint.spring = spring;
        joint.damper = damper;
        joint.massScale = massScale;
    }

    [Button]
    /// <summary>
    /// グラップルを開始する
    /// destinationの位置にSpringJointで接続し、物理演算を有効にする
    /// </summary>
    public void StartGrapple()
    {
        if (!gameObject.activeSelf) return;
        if (IsGrappling())
        {
            StopGrapple();
        }
        
        // グラップルポイントを目標地点に設定
        grapplePoint = destination.position;

        // SpringJointコンポーネントを動的に追加
        joint = _rigidbody.gameObject.AddComponent<SpringJoint>();

        // Rigidbodyの物理設定を有効化
        _rigidbody.isKinematic = false;
        _rigidbody.useGravity = true;
        _rigidbody.freezeRotation = true;

        // SpringJointの接続設定
        joint.autoConfigureConnectedAnchor = false;
        joint.connectedAnchor = grapplePoint;

        // 現在位置からグラップルポイントまでの距離を計算
        float distanceFromPoint = Vector3.Distance(_rigidbody.position, grapplePoint);

        // バネの距離制限を設定（実際の距離に係数を掛けて調整）
        joint.maxDistance = distanceFromPoint * maxDistance;
        joint.minDistance = distanceFromPoint * minDistance;

        // バネの物理パラメータを設定
        joint.spring = spring;
        joint.damper = damper;
        joint.massScale = massScale;
    }

    [Button]
    /// <summary>
    /// グラップルを終了する
    /// SpringJointを破棄し、Rigidbodyを運動学モードに戻す
    /// </summary>
    public void StopGrapple()
    {
        // Rigidbodyを運動学モードに変更（物理演算を無効化）
        _rigidbody.isKinematic = true;
        _rigidbody.useGravity = false;
        _rigidbody.freezeRotation = false;

        // SpringJointコンポーネントを削除
        Destroy(joint);
    }

    

    [Button]
    /// <summary>
    /// ケーブルを短縮する
    /// グラップルポイントに向かって力を加え、距離制限も更新する
    /// </summary>
    public void ShortenCable()
    {
        if (!IsGrappling()) return;

        // グラップルポイントに向かうベクトルを計算
        Vector3 directionToPoint = grapplePoint - _rigidbody.position;
        // 正規化したベクトルに推進力を掛けて力を加える
        _rigidbody.AddForce(directionToPoint.normalized * forwardThrustForce);

        // 現在距離を再計算
        float distanceFromPoint = Vector3.Distance(_rigidbody.position, grapplePoint);

        // 短縮された距離に基づいてSpringJointの距離制限を更新
        joint.maxDistance = distanceFromPoint * maxDistance;
        joint.minDistance = distanceFromPoint * minDistance;
    }


    [Button]
    /// <summary>
    /// ケーブルを延長する
    /// 現在距離にextendCableSpeedを加算して、SpringJointの距離制限を拡大する
    /// </summary>
    public void ExtendCable()
    {
        if (!IsGrappling()) return;
        
        // 現在距離に延長速度を加算
        float extendedDistanceFromPoint = Vector3.Distance(_rigidbody.position, grapplePoint) + extendCableSpeed;

        // 延長された距離に基づいてSpringJointの距離制限を更新
        joint.maxDistance = extendedDistanceFromPoint * maxDistance;
        joint.minDistance = extendedDistanceFromPoint * minDistance;
    }

    [Button]
    /// <summary>
    /// オブジェクトを右方向に押す力を加える
    /// </summary>
    public void AddForceRight()
    {
        _rigidbody.AddForce(_rigidbody.transform.right * horizontalThrustForce);
    }

    [Button]
    /// <summary>
    /// オブジェクトを左方向に押す力を加える
    /// </summary>
    public void AddForceLeft()
    {
        _rigidbody.AddForce(-_rigidbody.transform.right * horizontalThrustForce);
    }

    [Button]
    /// <summary>
    /// オブジェクトを前方向に押す力を加える
    /// </summary>
    public void AddForceForward()
    {
        _rigidbody.AddForce(_rigidbody.transform.forward * forwardThrustForce);
    }

    

    /// <summary>
    /// 現在グラップル中かどうかを判定する
    /// </summary>
    /// <returns>グラップル中の場合true</returns>
    public bool IsGrappling()
    {
        return joint != null;
    }

    /// <summary>
    /// 現在のグラップル接続ポイントを取得する
    /// </summary>
    /// <returns>グラップルポイントの世界座標</returns>
    public Vector3 GetGrapplePoint()
    {
        return grapplePoint;
    }

    /// <summary>
    /// グラップルガンの先端位置を取得する
    /// </summary>
    /// <returns>発射起点の世界座標</returns>
    public Vector3 GetGunTipPosition()
    {
        return origin.position;
    }
}