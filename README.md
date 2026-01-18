# Hiking Simulator

도보 여행을 테마로 한 3D 서바이벌 시뮬레이션 게임입니다.

## 게임 소개

플레이어는 도보 여행자가 되어 다양한 도로와 마을을 탐험합니다. 생존 시스템을 관리하고, 퀘스트를 수행하며, 아이템을 조합하여 여행을 완료하는 것이 목표입니다.

### 주요 기능

- **서바이벌 시스템**: 체력, 배고픔, 갈증을 관리하며 생존
- **동적 날씨**: 맑음, 비, 안개, 폭염 등 날씨가 생존에 영향
- **퀘스트 시스템**: 배달 퀘스트, 동행 퀘스트 수행
- **조합 시스템**: 재료를 조합하여 새로운 아이템 제작
- **인벤토리 & 장비**: 가방, 의류, 신발 장착으로 능력치 변화
- **상점 시스템**: 마을에서 아이템 구매/판매
- **NPC 동행**: 퀘스트를 통해 NPC와 함께 여행

## 기술 스택

| 구분 | 기술 |
|------|------|
| 엔진 | Unity 2022.3.62f3 (LTS) |
| 렌더링 | Universal Render Pipeline (URP) |
| 오디오 | DarkTonic MasterAudio |
| 플랫폼 | Steam (PC) |
| 언어 | C# |

## 아키텍처

### 싱글톤 기반 매니저 시스템

`SingletonBase<T>`를 상속받아 씬 전환에도 유지되는 매니저 클래스들로 구성:

```
GameManager       - 게임 상태 관리
UIManager         - UI 스택 및 화면 관리
InventoryManager  - 아이템 및 인벤토리
QuestManager      - 퀘스트 진행 관리
WeatherManager    - 날씨 및 시간
CombinationManager - 조합 레시피
```

### 이벤트 기반 통신

`InputManager`가 입력 이벤트를 브로드캐스트하여 시스템 간 결합도 최소화:

```csharp
InputManager.instance.OnInteractKeyPressed += HandleInteraction;
```

### ScriptableObject 기반 데이터

아이템, 레시피 등 게임 데이터를 ScriptableObject로 관리하여 에디터에서 쉽게 수정 가능

## 프로젝트 구조

```
Assets/
├── 00_Scenes/           # 게임 씬 (메인, 도로, 마을)
├── 01_Scripts/          # C# 스크립트
│   ├── Base/            # 싱글톤, 게임매니저
│   ├── PlayerScripts/   # 플레이어 이동, 상태, 생존
│   ├── InventoryScripts/# 인벤토리 시스템
│   ├── NPC/             # NPC 및 퀘스트
│   ├── Combination/     # 조합 시스템
│   └── Weather/         # 날씨 시스템
├── 03_Prefabs/          # 프리팹
└── 08_ScriptableObject/ # 아이템 데이터
```

## 스크린샷

<!-- 스크린샷 추가 예정 -->

## 조작법

| 키 | 동작 |
|----|------|
| WASD | 이동 |
| E | 상호작용 |
| I | 인벤토리 |
| F | 아이템 줍기 |
| Q | 퀘스트 목록 |
| R | 휴식 |
| ESC | 일시정지 |
