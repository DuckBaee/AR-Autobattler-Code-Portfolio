# External Dependencies

이 Repository는 코드 검토용이며 독립적으로 컴파일하거나 실행하는 것을 목표로 하지 않습니다. 생략된 의존성을 Stub, Mock, Interface 또는 재구현 코드로 대체하지 않습니다.

## Source Dependency Map

| 전시 코드 | 주요 의존성 | 원본 프로젝트에서의 역할 | Repository 처리 |
|---|---|---|---|
| `FindAllyWithLowestHealthRatio` | Unity Behavior, UnityEngine, `Character` | Tag 후보의 체력과 최대 체력 조회 | Target Selection Source만 공개 |
| `ActionAttackAction` | Unity Behavior, `Character` | Agent/Target 검증 및 `Attack` 호출 | 연결 코드만 공개 |
| `State` | Unity Behavior Blackboard | Chase/Attack/Idle 상태 값 | 그대로 공개 |
| `Healer` | `Character`, `BehaviorGraphAgent`, Physics | Runtime 변수 설정 및 범위 회복 | Unit 구현 맥락으로 공개 |
| `ARObjectPlacement` | AR Foundation, GameManager | Plane Raycast와 게임 상태 연결 | AR 통합 코드만 공개 |

## 공개하지 않은 프로젝트 시스템

### Character

- Health, MaxHealth, AttackPower, AttackRange 제공
- `Attack(Character target)` 다형성 API 제공
- Damage와 Heal 처리
- UI, VFX, Sound 등 여러 표현 시스템에 의존

### GameManager

- MapPlace, Tutorial, GameStart, GameOver, GameClear 상태 관리
- AR 배치 완료 상태와 게임 진행 연결
- 각종 UI와 VFX 참조 보유

### SoundManager 및 MonsterSpawner

- 전투 효과음과 Monster 생명주기 지원
- 선별한 AI/AR 문제 해결의 핵심이 아니므로 Source에서 제외

## Unity 제공 기능

- Unity Behavior
- AI Navigation
- AR Foundation
- ARCore XR Plugin
- XR Origin 및 XR Interaction Toolkit 구성
- Unity Input System

Unity 제공 Node와 Runtime 기능은 직접 작성한 코드로 분류하지 않습니다. 이 Repository가 강조하는 부분은 해당 기능을 게임 규칙과 Gameplay 흐름에 연결한 Custom 코드입니다.

