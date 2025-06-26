# Unity Spring System プロジェクト
必ず日本語で回答してください。

## プロジェクト概要
このプロジェクトはUnityで物理的なスプリングシステムを実装するプロジェクトです。

## プロジェクト構造
```
Assets/
├── Scripts/           # C#スクリプトファイル
├── Prefabs/          # プレハブファイル
├── Materials/        # マテリアルファイル
├── Textures/         # テクスチャファイル
├── Scenes/           # シーンファイル
└── Editor/           # エディタ拡張スクリプト
```

## 開発環境
- Unity 2022.3 LTS以上推奨
- C# 8.0以上
- Visual Studio 2022またはVisual Studio Code

## ビルド・実行方法
1. Unityエディタでプロジェクトを開く
2. 適切なシーンを選択
3. Playボタンでゲームモードに入る
4. ビルドする場合は File > Build Settings から対象プラットフォームを選択

## テスト
- Unity Test Runnerを使用してテストを実行
- Window > General > Test Runner からテストを実行可能

## コーディング規約
- C#の標準的な命名規則に従う
- クラス名：PascalCase
- メソッド名：PascalCase
- 変数名：camelCase
- プライベートフィールド：_camelCase（アンダースコア付き）

## 注意事項
- スクリプトの変更後はUnityエディタでの再コンパイルが必要
- シーンの変更は必ず保存すること
- プレハブの変更は慎重に行うこと

## 開発者向けメモ
- MonoBehaviourを継承したスクリプトはGameObjectにアタッチして使用
- Update(), Start(), Awake()などのUnityイベント関数を適切に使い分ける
- パフォーマンスを考慮してFixedUpdate()を物理演算に使用