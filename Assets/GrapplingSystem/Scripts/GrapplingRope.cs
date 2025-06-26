using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// グラップリングロープの視覚表現を管理するクラス
/// LineRendererとSpringシステムを使用して動的なロープアニメーションを描画します
/// </summary>
[RequireComponent(typeof(LineRenderer))]
public class GrapplingRope : MonoBehaviour
{
    /// <summary>ロープの物理的な振動を制御するSpringシステム</summary>
    private Spring spring;
    /// <summary>ロープの描画に使用するLineRenderer</summary>
    private LineRenderer lr;
    /// <summary>現在のグラップルポイント位置（補間用）</summary>
    private Vector3 currentGrapplePosition;
    /// <summary>グラップル状態を提供するインターフェース</summary>
    private IGrappleDrawer grapplingGun;
    /// <summary>ロープの品質（頂点数）</summary>
    [SerializeField, Tooltip("バネのラインの滑らかさ")] int quality = 500;
    /// <summary>バネの減衰係数</summary>
    [SerializeField, Tooltip("減衰係数 値が大きいほど振動が早く収束する")] float damper = 14f;
    /// <summary>バネの強度（バネ定数）</summary>
    [SerializeField, Tooltip("スプリングの強度（バネ定数）値が大きいほど強い力で目標値に向かう")]
    float strength = 800f;
    /// <summary>初期速度</summary>
    [SerializeField, Tooltip("現在の速度")] float velocity = 15f;
    /// <summary>波の数</summary>
    [SerializeField] float waveCount = 3;
    /// <summary>波の高さ</summary>
    [SerializeField] float waveHeight = 1;
    /// <summary>波の影響度を制御するカーブ</summary>
    [SerializeField] AnimationCurve affectCurve;

    /// <summary>
    /// 初期化処理
    /// 必要なコンポーネントの取得とSpringシステムの設定を行う
    /// </summary>
    void Awake()
    {
        // IGrappleDrawerインターフェースを持つコンポーネントを取得
        TryGetComponent(out grapplingGun);

        // LineRendererコンポーネントを取得
        lr = GetComponent<LineRenderer>();
        // Springシステムを初期化
        spring = new Spring();
        spring.SetTarget(0);
    }

    /// <summary>
    /// 毎フレーム終了時にロープを描画
    /// LateUpdateを使用して他の処理の後に描画を実行
    /// </summary>
    void LateUpdate()
    {
        DrawRope();
    }

    public void RestSpring()
    {
        // 現在位置をガンの先端位置に設定
        currentGrapplePosition = grapplingGun.GetGunTipPosition();
        // Springシステムをリセット
        spring.Reset();
        // LineRendererの頂点数を0にしてロープを非表示
        if (lr.positionCount > 0)
            lr.positionCount = 0;
    }

    public void InitializeSpring()
    {
        // 初回描画時の設定
        if (lr.positionCount == 0)
        {
            // 初期速度を設定
            spring.SetVelocity(velocity);
            // LineRendererの頂点数を設定
            lr.positionCount = quality + 1;
        }
    }

    void UpdateSpring(float deltaTime)
    {
        // Springシステムのパラメータ更新
        spring.SetDamper(damper);
        spring.SetStrength(strength);
        spring.Update(deltaTime);

        // グラップルポイントとガンの先端位置を取得
        var grapplePoint = grapplingGun.GetGrapplePoint();
        var gunTipPosition = grapplingGun.GetGunTipPosition();
        // ロープの方向に対して垂直な上方向ベクトルを計算
        var up = Quaternion.LookRotation((grapplePoint - gunTipPosition).normalized) * Vector3.up;

        // 現在のグラップル位置を滑らかに補間
        currentGrapplePosition = Vector3.Lerp(currentGrapplePosition, grapplePoint, deltaTime * 12f);

        // ロープの各頂点を計算
        for (var i = 0; i < quality + 1; i++)
        {
            // インデックスが範囲外になるのを防ぐ
            if (i >= lr.positionCount)
                break;

            // 0から1の範囲で正規化された位置
            var delta = i / (float)quality;
            // 正弦波とSpringの値を使用して波状のオフセットを計算
            var offset = up * waveHeight * Mathf.Sin(delta * waveCount * Mathf.PI) * spring.Value *
                         affectCurve.Evaluate(delta);

            // ガンの先端からグラップルポイントまでの直線上の位置にオフセットを加算
            lr.SetPosition(i, Vector3.Lerp(gunTipPosition, currentGrapplePosition, delta) + offset);
        }
    }

    /// <summary>
    /// ロープの描画処理
    /// グラップル状態に応じてロープの形状を動的に計算し、LineRendererに設定する
    /// </summary>
    void DrawRope()
    {
        // グラップル中でない場合はロープを描画しない
        if (!grapplingGun.IsGrappling())
        {
            RestSpring();
            return;
        }
        else
        {
            InitializeSpring();
        }

        UpdateSpring(Time.deltaTime);
    }
}