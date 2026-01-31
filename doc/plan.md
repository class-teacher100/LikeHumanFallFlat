# Human: Fall Flat風キャラクター移動 — 実装計画

## 概要

Unity 6 (6000.1.9f1) + URP 17.1.0 環境で、Human: Fall Flat風の物理ベースラグドールキャラクターを実装する。

## Phase 0: プロジェクトセットアップ

- `git init` + Unity用 `.gitignore` 作成
- `Assets/Scripts/` 以下にフォルダ構成作成: `Character/`, `Camera/`, `Input/`, `Physics/`, `Environment/`, `Editor/`
- `InputSystem_Actions.inputactions` に `GrabLeft`(左クリック)、`GrabRight`(右クリック) アクション追加

## Phase 1: ラグドールキャラクター構築

プリミティブ(Capsule/Sphere)で構成した人型ラグドールを作成。

**身体構造** (Pelvisをルートとした階層):
```
Pelvis → Torso → Head
              → LeftUpperArm → LeftLowerArm → LeftHand
              → RightUpperArm → RightLowerArm → RightHand
       → LeftUpperLeg → LeftLowerLeg → LeftFoot
       → RightUpperLeg → RightLowerLeg → RightFoot
```

各パーツに Rigidbody + Collider + ConfigurableJoint を設定。Joint の spring/damper で直立姿勢を維持。

**スクリプト:**

| スクリプト | 役割 |
|---|---|
| `CharacterDefinition.cs` | ScriptableObject: 質量・寸法・Joint設定値・移動パラメータ |
| `BodyPart.cs` | 個々のパーツ (Rigidbody/Joint参照、力・トルク適用) |
| `RagdollCharacter.cs` | 全パーツ参照を保持、`BuildRagdoll()` でランタイム生成 |
| `BalanceController.cs` | 胴体/骨盤にトルクを加えて直立を維持 |

## Phase 2: 物理ベース移動

入力に基づき Pelvis の Rigidbody に力を加えて移動。

| スクリプト | 役割 |
|---|---|
| `PlayerInputHandler.cs` | Input System から入力値を読み取りキャッシュ |
| `GroundDetector.cs` | 足からの Raycast で接地判定 |
| `MovementController.cs` | カメラ方向基準で移動力を適用、最大速度制限 |
| `JumpController.cs` | 接地時に Pelvis/足へ上方向のインパルス |

## Phase 3: 三人称カメラ

マウスで回転するオービットカメラ。

| スクリプト | 役割 |
|---|---|
| `ThirdPersonCamera.cs` | マウス入力で回転、キャラクター追従、垂直角度制限 |
| `CameraCollision.cs` | SphereCast で壁貫通を防止 |

## Phase 4: 腕の制御とグラブ

HFF の核心機能。左右マウスボタンで対応する腕をカメラ方向に伸ばし、物をつかむ。

| スクリプト | 役割 |
|---|---|
| `ArmController.cs` | ConfigurableJoint の target rotation で腕を目標方向へ誘導 |
| `ArmReachTarget.cs` | カメラ方向から左右腕の到達目標位置を計算 |
| `GrabController.cs` | 手の周辺を検出、FixedJoint で接続/解除 |
| `Grabbable.cs` | つかめるオブジェクトのマーカーコンポーネント |

## Phase 5: テスト環境

| スクリプト | 役割 |
|---|---|
| `Platform.cs` | 静止/移動プラットフォーム |
| `PhysicsDebugger.cs` | Joint・力・接地の Gizmos 表示 |
| `TestSceneBuilder.cs` | ランタイムにテストシーンを構築 |
| `SetupHelper.cs` (Editor) | レイヤー設定、アセット作成、シーン初期化メニュー |

**シーン構成:**
- 地面 (Plane) + Ground レイヤー
- 段差テスト (0.5m, 1m, 1.5m)
- つかめる箱・球体 (Grabbable レイヤー)
- 登れる壁 (等間隔の突起)

## レイヤー設定

| レイヤー番号 | 名前 | 用途 |
|---|---|---|
| 6 | Ground | 地面・プラットフォーム |
| 7 | Character | キャラクターパーツ (パーツ同士の衝突無効化) |
| 8 | Grabbable | つかめるオブジェクト |

## 物理パラメータ初期値

| パラメータ | 値 |
|---|---|
| Pelvis 質量 | 15 kg |
| Torso 質量 | 10 kg |
| Head 質量 | 5 kg |
| 腕パーツ質量 | 1〜3 kg |
| 脚パーツ質量 | 2〜5 kg |
| 背骨 Joint spring / damper | 2500 / 150 |
| 肩 Joint spring / damper | 800 / 80 |
| 膝 Joint spring / damper | 1000 / 100 |
| バランストルク / ダンピング | 3000 / 200 |
| 移動力 | 50 N |
| ジャンプインパルス | 300 N |
| 最大速度 | 5 m/s |
| グラブ FixedJoint breakForce | 2000 N |

全パラメータは `CharacterDefinition` ScriptableObject に格納し、Inspector から調整可能。
