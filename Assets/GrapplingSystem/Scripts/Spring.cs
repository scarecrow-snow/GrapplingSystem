using UnityEngine;

/// <summary>
/// 物理的なスプリング（バネ）の挙動をシミュレートするクラス
/// UIアニメーションやオブジェクトの滑らかな移動に使用
/// </summary>
public class Spring
{
    /// <summary>
    /// スプリングの強度（バネ定数）
    /// 値が大きいほど強い力で目標値に向かう
    /// </summary>
    private float strength;
    
    /// <summary>
    /// 減衰係数
    /// 値が大きいほど振動が早く収束する
    /// </summary>
    private float damper;
    
    /// <summary>
    /// 目標値
    /// スプリングが向かう先の値
    /// </summary>
    private float target;
    
    /// <summary>
    /// 現在の速度
    /// </summary>
    private float velocity;
    
    /// <summary>
    /// 現在の値
    /// </summary>
    private float value;

    /// <summary>
    /// スプリングの物理演算を実行
    /// フレームごとに呼び出してスプリングの状態を更新する
    /// </summary>
    /// <param name="deltaTime">前フレームからの経過時間</param>
    public void Update(float deltaTime) {
        // 目標値への方向を計算
        var direction = target - value >= 0 ? 1f : -1f;
        // 距離に応じた力を計算
        var force = Mathf.Abs(target - value) * strength;
        // 力と減衰を考慮して速度を更新
        velocity += (force * direction - velocity * damper) * deltaTime;
        // 速度から位置を更新
        value += velocity * deltaTime;
    }

    /// <summary>
    /// スプリングの状態をリセット
    /// 速度と値を0に戻す
    /// </summary>
    public void Reset() {
        velocity = 0f;
        value = 0f;
    }
        
    /// <summary>
    /// 現在の値を設定
    /// </summary>
    /// <param name="value">設定する値</param>
    public void SetValue(float value) {
        this.value = value;
    }
        
    /// <summary>
    /// 目標値を設定
    /// </summary>
    /// <param name="target">目標値</param>
    public void SetTarget(float target) {
        this.target = target;
    }

    /// <summary>
    /// 減衰係数を設定
    /// </summary>
    /// <param name="damper">減衰係数（0以上の値）</param>
    public void SetDamper(float damper) {
        this.damper = damper;
    }
        
    /// <summary>
    /// スプリングの強度を設定
    /// </summary>
    /// <param name="strength">スプリング強度（0以上の値）</param>
    public void SetStrength(float strength) {
        this.strength = strength;
    }

    /// <summary>
    /// 速度を設定
    /// </summary>
    /// <param name="velocity">設定する速度</param>
    public void SetVelocity(float velocity) {
        this.velocity = velocity;
    }
        
    /// <summary>
    /// 現在の値を取得
    /// </summary>
    public float Value => value;
}