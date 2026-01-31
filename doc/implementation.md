# 実装詳細

## ファイル構成

```
Assets/Scripts/
├── Character/
│   ├── ArmController.cs        — 腕のJoint制御
│   ├── ArmReachTarget.cs       — カメラ方向からの到達目標計算
│   ├── BalanceController.cs    — 直立維持トルク
│   ├── BodyPart.cs             — 身体パーツ基本コンポーネント
│   ├── CharacterDefinition.cs  — パラメータ定義 (ScriptableObject)
│   ├── Grabbable.cs            — つかめるオブジェクトマーカー
│   ├── GrabController.cs       — 掴み/離し制御
│   ├── JumpController.cs       — ジャンプ制御
│   ├── MovementController.cs   — 移動制御
│   └── RagdollCharacter.cs     — ラグドール本体管理
├── Camera/
│   ├── CameraCollision.cs      — カメラ壁貫通防止
│   └── ThirdPersonCamera.cs    — 三人称オービットカメラ
├── Editor/
│   └── SetupHelper.cs          — エディタメニュー (レイヤー/アセット/シーン設定)
├── Environment/
│   ├── Platform.cs             — プラットフォーム (静止/移動)
│   └── TestSceneBuilder.cs     — テストシーンランタイム構築
├── Input/
│   └── PlayerInputHandler.cs   — Input System入力ラッパー
└── Physics/
    ├── GroundDetector.cs       — 接地判定
    └── PhysicsDebugger.cs      — 物理デバッグGizmos
```

合計: 18スクリプト

## 各スクリプトの実装概要

### CharacterDefinition.cs

`ScriptableObject` として全物理パラメータを一元管理。Inspector上で調整可能。

- 身体パーツごとの質量・寸法
- 各関節の spring / damper 値
- 移動力、ジャンプインパルス、最大速度
- バランストルク、グラブ破断力

メニュー `Assets > Create > Character > Character Definition` から作成可能。

### BodyPart.cs

各身体パーツにアタッチされるコンポーネント。

- `BodyPartType` enumで種別を識別
- Awake時に Rigidbody / ConfigurableJoint / Collider を自動取得
- `ApplyForce()`, `ApplyTorque()`, `SetJointTargetRotation()`, `SetJointDrive()` ヘルパーメソッド

### RagdollCharacter.cs

ラグドール全体を管理する中央コンポーネント。

- 15パーツ全てへの参照フィールド
- `BuildRagdoll()` メソッド: CharacterDefinition に基づきプリミティブからラグドールをランタイム生成
  - Capsule/Sphere プリミティブの生成
  - CapsuleCollider / SphereCollider の設定
  - Rigidbody の質量設定
  - ConfigurableJoint の接続・drive設定
  - パーツ間の衝突を `Physics.IgnoreCollision` で無効化
- Context Menu からエディタ上でも実行可能

### BalanceController.cs

pelvis と torso に対し、ワールドUpに整列させるトルクを毎 FixedUpdate で適用。

- `Vector3.Cross(currentUp, worldUp)` で補正軸を計算
- 角度に比例した補正トルク + 角速度に対するダンピングトルク

### PlayerInputHandler.cs

Unity Input System の `InputSystem_Actions` を使用。

- `MoveInput` (Vector2), `LookInput` (Vector2) を毎フレーム読み取り
- `JumpPressed`, `GrabLeftHeld`, `GrabRightHeld` をコールバックで管理
- `ConsumeJump()` で1回限りのジャンプ消費

起動時にカーソルをロック・非表示にする。

### GroundDetector.cs

左右の足から下方向へ Raycast し、Ground レイヤーとの衝突を検出。

- `IsGrounded` プロパティ: いずれかの足が接地していれば true
- `GroundNormal` プロパティ: 接地面の法線

### MovementController.cs

カメラの前方/右方向を水平面に投影し、入力に基づく方向を計算。

- 接地時のみ移動力を適用
- `pelvis.Rb` に `ForceMode.Force` で力を加える
- 水平速度が `maxSpeed` 未満の場合のみ力を追加

### JumpController.cs

ジャンプ入力 + 接地条件が揃った場合にインパルスを適用。

- Pelvis に60%、左右の足に各20% のインパルスを配分
- `ForceMode.Impulse` で瞬間的な力

### ThirdPersonCamera.cs

LateUpdate でカメラ位置を更新。

- マウスの Look 入力で yaw/pitch を更新
- pitch を -30° 〜 70° にクランプ
- ターゲット位置 (pelvis + offset) から distance 分後退した位置を計算
- `Vector3.Lerp` でスムージング、`LookAt` でターゲットを注視

### CameraCollision.cs

LateUpdate (ThirdPersonCamera の後) で SphereCast を実行。

- ターゲット → カメラ方向へ SphereCast
- 衝突があればカメラ位置を衝突点手前に移動

### ArmReachTarget.cs

カメラの向きとキャラクター位置から、左右の腕の到達目標位置を計算。

- `GetTargetPosition(isLeft)`: torso位置 + カメラ前方 × reachDistance + 左右オフセット
- `GetReachDirection(isLeft)`: 肩から目標位置への方向ベクトル

### ArmController.cs

FixedUpdate で左右の腕を制御。

- グラブボタン押下時: 肩・肘の Joint drive を強化し、target rotation をカメラ方向へ設定
- ボタン非押下時: Joint drive をデフォルト値に戻し、target rotation を identity (休息姿勢) に復帰

### GrabController.cs

FixedUpdate で左右の手の掴み状態を管理。

- **掴み**: `Physics.OverlapSphere` で手の近くのコライダーを検出。自身の BodyPart を除外し、Grabbable コンポーネント付き or 静的オブジェクトに FixedJoint を接続
- **離し**: FixedJoint を Destroy
- **自然破断**: `breakForce` (2000N) を超えた場合、Joint が自動破壊

### Grabbable.cs

つかめるオブジェクトに付与するマーカーコンポーネント。Rigidbody を自動取得。

### Platform.cs

プラットフォームコンポーネント。

- `_isMoving` が true の場合、Kinematic Rigidbody を追加し `MovePosition` で PingPong 移動
- 方向・距離・速度を Inspector から設定

### TestSceneBuilder.cs

Start() でテストシーン全体をランタイム構築。

- レイヤー衝突設定
- 地面 (100m × 100m Plane)
- 段差 (0.5m, 1.0m, 1.5m の Cube)
- つかめるオブジェクト (箱3個 + 球2個、Grabbable + Rigidbody 付き)
- 登攀壁 (4行×3列の突起付き壁)
- プレイヤーキャラクター (全コンポーネント接続済み)

リフレクションで `[SerializeField]` フィールドにランタイム注入。

### SetupHelper.cs (Editor)

`Game` メニューに3つのコマンドを追加:

- **Setup Layers**: TagManager に Ground(6), Character(7), Grabbable(8) を設定
- **Create Character Definition Asset**: `Assets/Settings/` にデフォルト設定アセット作成
- **Setup Test Scene**: 上記2つを実行 + TestSceneBuilder をシーンに配置

### PhysicsDebugger.cs

OnDrawGizmos で物理状態を可視化:

- **Joints**: アンカー位置にワイヤースフィア + 接続線
- **速度**: 各パーツの velocity をレイ表示
- **接地**: 足からの下方レイ (緑=接地 / 赤=空中)

## Input System 変更

`InputSystem_Actions.inputactions` の Player マップに2アクション追加:

| アクション | タイプ | バインド |
|---|---|---|
| GrabLeft | Button | Mouse/leftButton, Gamepad/leftTrigger |
| GrabRight | Button | Mouse/rightButton, Gamepad/rightTrigger |

## セットアップ手順

1. Unity でプロジェクトを開く
2. メニュー `Game > Setup Test Scene` を実行
3. Play モードに入る → テスト環境とキャラクターが自動生成される

## 操作方法

| 入力 | 動作 |
|---|---|
| WASD | 移動 |
| マウス移動 | カメラ回転 |
| Space | ジャンプ |
| 左クリック (長押し) | 左腕を伸ばす / 掴む |
| 右クリック (長押し) | 右腕を伸ばす / 掴む |
