# TowerDefence (임시 제목)

> Unity로 제작한 타워 디펜스 게임  
> 제한된 골드와 포탑 슬롯을 활용해 적 웨이브를 막아내는 전략형 게임 프로젝트입니다.

<!-- TODO: 스크린샷 있으면 여기에 추가
![screenshot](./Docs/title.png)
-->

---

## Table of Contents

1. [프로젝트 개요 (Overview)](#프로젝트-개요-overview)
2. [게임 규칙 & 시스템 (Game Rules & System)](#게임-규칙--시스템-game-rules--system)
3. [플레이 방법 (How to Play)](#플레이-방법-how-to-play)
4. [구현 구조 (Implementation Details)](#구현-구조-implementation-details)
1. [참고 자료 (References)](#참고-자료-references)

---

## 프로젝트 개요 (Overview)

### 1) 한 줄 소개

- **장르**: 타워 디펜스 (Tower Defence)
- **엔진**: Unity `TODO: 사용한 버전 (예: 6000.0.43f1 LTS)`
- **플랫폼**: PC (Windows) 기준

### 2) 프로젝트 목표

- 단순히 포탑을 배치하는 것을 넘어,  
  **골드 관리, 포탑 제한 수, 판매/재배치** 등의 요소를 통해  
  플레이어가 전략적으로 고민하게 만드는 것이 목표입니다.
- 학과 과제/프로젝트로서,
  - Unity를 이용한 게임 루프 설계
  - 싱글톤 매니저, Node 기반 타일 시스템
  - 자원(골드) 관리 및 UI 갱신
  를 직접 구현하는 것에 초점을 두었습니다.

### 3) 핵심 특징

- 최대 **`maxTurrets` 개**의 포탑만 설치 가능 → 무한 설치 불가, 전략 필요
- 포탑 판매 시 **구매 비용의 일정 비율(예: 50%) 환불**
- 마우스로 노드를 클릭해 포탑을 설치하거나, 이미 설치된 포탑을 선택 후 **X 키로 판매**
- ESC 키로 현재 선택 상태 해제 (포탑 선택 & 건설 모드 모두 해제)
---

## 게임 규칙 & 시스템 (Game Rules & System)

> 아래 내용은 현재 구현 기준/혹은 목표 기준입니다. 실제와 다르면 TODO 수정해주세요.

### 1) 자원 (골드 Gold)

- 플레이어는 시작 시 일정량의 골드를 보유합니다.  
  예: `TODO: 시작 골드 (예: 100)`
- 포탑을 설치할 때마다 해당 포탑의 **비용만큼 골드가 감소**합니다.
- 골드가 부족하면 해당 포탑은 설치할 수 없습니다.
- 포탑 판매 시:  
  `환불 골드 = 포탑 가격 × 환불 비율 (sellRefundRate)`  
  예: 50골드 포탑, 0.5 비율 → 판매 시 25골드 환불

### 2) 포탑 종류 (Turrets)

현재 세 가지 종류의 포탑을 지원합니다:

- **Standard Turret**
  - 비용: `standardTurretCost`(50)
  - 기본 단일 타겟 공격

- **Missile Launcher**
  - 비용: `missileLauncherCost` (90)
  - 고화력, 느린 공격 속도 / 사거리가 김

- **Artillery**
  - 비용: `artilleryCost` (예: 70)
  - 기본사거리, 빠른공격속도, 낮은 공격력


### 3) 포탑 설치 제한

- **`maxTurrets` 개**까지만 설치 가능 (8개,16개)
- `currentTurretCount`를 통해 현재 설치된 포탑 수를 관리
- 제한 개수를 넘으면 설치불가

### 4) 포탑 선택 & 판매

- **노드를 클릭**:
  - 비어 있는 노드 + 건설 모드 → 포탑 설치
  - 이미 포탑이 있는 노드 → 해당 포탑을 "선택" 상태로 전환
- 선택된 포탑은 `selectedColor`로 표시 (타워 사거리 표시)
- **X 키**를 누르면:
  - 현재 선택된 노드의 포탑을 판매(Sell)  
  - 골드 일부 환불 + 포탑 개수 감소
- **ESC 키**를 누르면:
  - 선택된 포탑 해제
  - 건설할 포탑 선택도 해제 (건설 모드 종료)

---

## 플레이 방법 (How to Play)

### 1) 기본 조작

- **마우스 좌클릭**
  - 비어 있는 타일(Node)을 클릭 → 선택한 포탑 설치 시도
  - 이미 포탑이 설치된 타일 클릭 → 해당 포탑 선택
- **키보드**
  - `X` : 선택된 포탑 판매
  - `ESC` : 선택 해제 & 건설 모드 해제

### 2) 게임 흐름

1. 게임 시작 → 초기 골드 & 맵 로딩
2. UI에서 건설할 포탑 종류 선택  
   (예: Standard / Missile / Artillery 버튼 클릭 시 `BuildManager.SetTurretToBuild(...)` 호출)
3. 맵의 빈 Node를 클릭하여 포탑 설치
4. 적 웨이브가 들어오는 동안:
   - 골드를 모으고
   - 필요한 위치에 추가 포탑 설치
   - 비효율적인 포탑은 판매 후 재배치
5. 모든 웨이브를 막거나, 기지가 파괴되면 게임 종료  
   (TODO: 실제 승리/패배 조건 작성)



## 구현 구조 (Implementation Details)

### 1) 주요 스크립트 개요

#### `BuildManager` (buildmanager.cs)

- **역할**: 포탑 건설/선택/판매를 총괄 관리하는 싱글톤 매니저
- 주요 기능:
  - 설치 가능한 포탑 프리팹 및 비용 관리
  - 현재 설치된 포탑 개수 / 최대 포탑 개수 관리
  - 포탑 판매 시 환불 금액 계산 (`sellRefundRate`)
  - 선택된 노드(Node) 상태 관리
  - 키 입력(`X`, `ESC`) 처리

#### `Node` (Node.cs)

- **역할**: 실제 포탑이 설치되는 타일(칸)을 표현하는 스크립트
- 주요 기능:
  - 마우스 클릭 시 포탑 설치 or 포탑 선택
  - 선택 시 색 변경 (`selectedColor`)
  - 호버 시 임시 색 변경 (`hoverColor`) – 단, 선택과는 별개
  - 설치된 포탑에 대한 참조 관리 (`turret`)
  - `SellTurret()`를 통해 포탑 판매 로직 수행

#### `Gold` (Gold.cs)

- **역할**: 플레이어 골드 관리 싱글톤 (가정)
- 주요 메서드 예시:
  - `bool SpendGold(int amount)`
  - `void AddGold(int amount)`
  - 현재 골드를 UI와 연동

#### 기타 스크립트 (TODO)

- `Enemy`, `WaveSpawner`, `GameManager` 등
  - 실제 구현된 스크립트 이름과 역할을 간단히 bullet로 정리

### 2) 간단한 흐름도 (의사 코드)

```text
[Player] 
  → UI에서 Turret 타입 선택
    → BuildManager.SetTurretToBuild(prefab)

  → Node 클릭
    → 이미 turret가 있다면 → BuildManager.SelectTurret(node)
    → turret가 없고, BuildManager에 선택된 turret가 있다면
         → Gold.SpendGold(cost) 성공 시
              → turret Instantiate
              → BuildManager.OnTurretBuilt()
```
### 참고 자료 (References)
https://www.youtube.com/watch?v=beuoNuK2tbk

BGM:https://www.youtube.com/watch?v=V71z8yFKjk4&list=PL22iamDQYGnO5NxZ9vo6SU88yEilFlyMJ&index=2

사용된 에셋: 
https://assetstore.unity.com/packages/3d/props/weapons/fatty-poly-turret-free-155251
https://assetstore.unity.com/packages/3d/props/weapons/fatty-poly-turret-part-2-free-159989

cursor Ai를 UNITY와 연동해 사용하여 코드작성에 있어 도움을 받음