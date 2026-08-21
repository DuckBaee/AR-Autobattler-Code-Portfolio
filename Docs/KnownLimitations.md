# Known Limitations

이 문서는 원본 코드의 동작을 수정하지 않고, 코드 리뷰 시 확인해야 할 제한사항을 기록합니다.

## ActionAttackAction

- Healer 여부를 GameObject 이름 문자열 `"Healer"`로 판별합니다.
- 실제 Prefab 또는 Instantiate 후 이름과 일치하지 않으면 Healer 전용 분기가 실행되지 않을 수 있습니다.
- `OnStart`와 `OnUpdate`의 null Target 처리 결과가 일관되지 않습니다.
- Target이 공격 범위를 벗어나면 `Failure`를 반환하고 Graph의 거리 Guard 및 State 흐름에 의존합니다.

## Healer Target과 실제 회복

- Custom Action은 체력 비율이 가장 낮은 Ally를 Blackboard Target으로 선정합니다.
- 실제 `Healer` 코드는 일정 주기로 `Physics.OverlapSphere`를 실행해 주변 아군을 범위 회복합니다.
- 따라서 현재 구조는 “선택한 단일 Target을 직접 회복”이라기보다 “지원 우선 대상 방향으로 이동한 뒤 주변을 범위 회복”으로 설명하는 것이 정확합니다.

## Target Search

- `FindAllyWithLowestHealthRatio`는 실행할 때마다 `GameObject.FindGameObjectsWithTag`로 전체 후보를 검색합니다.
- Healer Graph에서 호출 주기가 충분히 제한되지 않으면 Unit 수에 따라 탐색 비용이 커질 수 있습니다.
- 일반 공격 Target 재탐색은 Custom Node가 아니라 반복되는 Unity 기본 `Find Closest With Tag` Node가 담당합니다.
- 명시적인 Target 생존 확인 Node는 확인되지 않았습니다.

## Dependencies

- 원본 `Character`는 GameManager, SoundManager, UI, VFX와 강하게 연결되어 있습니다.
- `ARObjectPlacement`는 GameManager의 게임 상태를 참조합니다.
- 이 Repository는 해당 의존성을 제거하거나 대체 구현하지 않으므로 독립 컴파일되지 않습니다.

## Evidence Scope

- 현재 Git에는 최종 파일이 포함된 Commit은 있지만, 개발 중간 과정은 남아 있지 않습니다.
- 기본 Node에서 Custom Action으로 변화한 과정을 Git Evolution으로 주장하지 않습니다.
- 실제 모바일 기기 동작과 성능은 별도 실행 영상이 있을 때만 결과로 제시합니다.

