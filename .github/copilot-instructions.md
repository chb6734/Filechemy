# C# (.NET Framework 3.5) 코드 리뷰 가이드

## Steps
### 1) Code Analysis
- **구조/모듈화**: 계층 분리(도메인/응용/인프라), 단일 책임, 의존성 방향(상→하) 준수 여부.
- **가독성**: .NET 네이밍 규칙(PascalCase/ camelCase), 짧고 명확한 메서드, 의미 있는 네이밍, 주석은 “왜”에 집중.
- **언어 기능(C# 3.0) 활용**: `var` 적절성, 람다/식 트리, **LINQ to Objects/Entities** 남용 여부, **확장 메서드**의 위치(Static class/this 수식).
- **예외·자원관리**: `IDisposable`/`using` 패턴, 예외 전파/로깅 전략, COM/Interop 해제(`Marshal.ReleaseComObject`).
- **성능**: 빈번한 할당/박싱, 문자열 결합(`StringBuilder`), 긴 루프에서 LINQ vs for, 컬렉션 선택(List/Dictionary/SortedDictionary).
- **스레딩/동시성**: TPL/async 없음 → `ThreadPool`/`BackgroundWorker`/`BeginInvoke`/`Control.Invoke` 패턴 적정성, `lock`/`Monitor`/`Interlocked` 사용.
- **플랫폼 특성**: WinForms/WPF UI 스레드 규칙(Dispatcher/InvokeRequired), WCF 구성, ASP.NET WebForms/MVC1 패턴 준수.
- **데이터 접근**: ADO.NET/EF 1.0/LINQ to SQL 쿼리 추상화·연결수명·트랜잭션 범위, `using`으로 커넥션·리더 정리.
- **보안/국제화**: 입력검증, 암호 저장 정책(절대 평문 X), `CultureInfo`/`StringComparison`, `DateTimeOffset`(3.5 지원) 활용.
- **배포/구성**: `app.config`의 설정 분리, AnyCPU/x86 타깃 적정성, Strong Name/GAC 필요성 검토, FxCop/StyleCop 적용.

### 2) Error Detection
- **컴파일/설계 경고**: 사용되지 않는 변수, 가상/재정의 누락, `IDisposable` 미해제, 이벤트 누수, 다중 스레드 안전성.
- **런타임 리스크**: Null 참조, 컬처 의존 비교, SQL 인젝션, 교착상태/경쟁조건, IO/네트워크 예외 미처리.
- **버전 의존**: Office Interop/3rd-party DLL 버전 상이, AnyCPU에서의 COM 32/64bit 충돌.

### 3) Improvement Suggestions
- **리팩터링**: 큰 메서드 분해, 조건문 단순화, 전략/템플릿/팩토리 패턴 도입.
- **성능 최적화**: 핫패스에서 LINQ→for 전환, `ToList()` 불필요 제거, 박싱 방지, `StringBuilder`, 컬렉션/딕셔너리 키 비교자 지정.
- **안정성**: `using` 철저, `try/catch` 범위 최소화+로깅, 재시도 정책(지수 백오프—직접 구현).
- **테스트 용이성**: 인터페이스 도입, new 분기 제거(팩토리), 시간/IO/시계 추상화.
- **가이드라인 준수**: .NET 디자인 가이드라인(이벤트, 속성, 예외)·FxCop 규칙 적용.

### 4) Conclusion
- 변경 필요/최소 변경/유지 가능 중 하나로 판정, 핵심 근거 요약.

---

## Output Format (리뷰 응답 템플릿)

**Analysis:**  
코드의 구조/가독성/성능/안정성/버전 적합성을 요약.

**Errors & Warnings:**  
- [경고/오류1]: 설명  
- [경고/오류2]: 설명

**Suggestions:**  
1) 제안 제목 — 구체적 조치(짧은 코드 예시 포함)  
2) …

**Conclusion:**  
- 결론(변경 필요/최소 변경/유지 가능) + 근거

---

## 체크리스트 (요약)

- [ ] **네이밍/스타일**: PascalCase/ camelCase, `private` 필드 `_camelCase`(팀 규칙 일관)  
- [ ] **메서드 크기/책임**: 30~50줄 내외, 한 가지 일  
- [ ] **LINQ 남용 방지**: 핫패스/할당 민감 경로에서는 루프 고려  
- [ ] **자원관리**: 모든 `IDisposable`은 `using`  
- [ ] **예외**: 의미 있는 메시지, 상위에서 로깅/정책 처리  
- [ ] **스레드 안전**: UI 스레드 접근은 `Invoke/Dispatcher`, 공유상태 보호  
- [ ] **문자열/컬처**: 비교 시 `StringComparison`, 출력 시 `CultureInfo`  
- [ ] **시간/시간대**: `DateTimeOffset` 우선(3.5 지원), 로컬/UTC 명확화  
- [ ] **데이터 접근**: 파라미터라이즈드 쿼리, 연결수명 최소, 트랜잭션 범위 명확  
- [ ] **설정 분리**: `app.config`/환경별 설정, 민감정보 암호화  
- [ ] **분석 도구**: FxCop/StyleCop, 경고를 오류로 처리(가능 시)

---

## 예시 (간단 시나리오)

### Example 1
**Analysis:**  
LINQ를 광범위하게 사용해 가독성은 좋으나, 대량 반복 구간에서 `ToList()`/`Select` 체이닝이 잦아 할당이 많습니다. 자원(`SqlConnection`)은 `using`으로 적절히 정리합니다.

**Errors & Warnings:**  
- 경고: `String.Concat` 루프 내 다중 호출 → 문자열 할당 증가

**Suggestions:**  
1) **핫패스 LINQ 최소화**  
   - 현재:
     ```csharp
     var names = users.Where(u => u.Active).Select(u => u.Name).ToList();
     ```
   - 제안(핫패스 한정):
     ```csharp
     var names = new List<string>();
     foreach (var u in users)
     {
         if (!u.Active) continue;
         names.Add(u.Name);
     }
     ```
2) **문자열 결합 최적화**
   ```csharp
   var sb = new StringBuilder();
   foreach (var s in parts) sb.Append(s);
   var result = sb.ToString();
   ```

**Conclusion:**  
**최소 변경 권장**. 가독성 유지 범위에서 핫패스 최적화만 적용하면 충분합니다.

---

### Example 2
**Analysis:**  
WinForms UI 스레드 규칙을 일부 위반합니다. 백그라운드 스레드에서 컨트롤 업데이트를 직접 호출합니다.

**Errors & Warnings:**  
- 오류 가능: `Cross-thread operation not valid` 예외

**Suggestions:**  
1) **UI 스레드 전환**  
   ```csharp
   if (this.InvokeRequired)
       this.Invoke(new Action(() => this.label1.Text = text));
   else
       this.label1.Text = text;
   ```
2) **BackgroundWorker 패턴 사용**  
   - `DoWork`에서 작업, `RunWorkerCompleted`에서 UI 업데이트.

**Conclusion:**  
**변경 필요**. UI 스레드 규칙 준수가 필수입니다.

---

## .NET 3.5 특화 베스트 프랙티스

- **언어 기능**: 자동구현 프로퍼티, 확장 메서드, 람다/LINQ는 “가독성 향상 목적”으로 사용. 성능 민감 구간은 프로파일링 후 선택.
- **동시성**: TPL/async 미지원 → `ThreadPool`, `BackgroundWorker`, `BeginInvoke/EndInvoke`, `IAsyncResult` 패턴 숙지. 공유 상태는 `lock`/`Interlocked`.
- **데이터 계층**: EF v1 또는 LINQ to SQL 사용 시, **지연 로딩/추적**로 인한 의도치 않은 쿼리 다발을 경계. 투명한 쿼리 경계(리포지토리/쿼리 객체) 확보.
- **WCF**: 바인딩/타임아웃/직렬화(DataContractSerializer) 점검, `using`으로 `ChannelFactory`/`ClientBase` 수명 관리.
- **Interop**: Office/COM 자원은 **반드시 해제**, RCW 누수 방지(`ReleaseComObject` 루프 + `GC.Collect()`는 최후수단).
- **국제화/로캘**: 비교 시 `StringComparison.Ordinal`/`OrdinalIgnoreCase`(정렬/검색은 문화권 비교), 포맷은 `CultureInfo`.
- **시간/타임존**: `DateTimeOffset` 도입(3.5), 저장은 UTC 선호, 변환 경계 명확.
- **로깅**: Enterprise Library Logging, log4net 등 검토. 예외 삼키지 말 것.
- **정적 분석**: FxCop/StyleCop, CA 규칙 기반으로 API 설계 가이드라인 준수.

---

## 미니 코드 스니펫 (자주 지적되는 포인트)

**IDisposable 패턴**
```csharp
sealed class MyResource : IDisposable
{
    private IntPtr _handle; // 예시
    private bool _disposed;

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    ~MyResource() { Dispose(false); }

    private void Dispose(bool disposing)
    {
        if (_disposed) return;
        if (disposing)
        {
            // Managed 해제
        }
        // Unmanaged 해제
        if (_handle != IntPtr.Zero) { /* free */ _handle = IntPtr.Zero; }
        _disposed = true;
    }
}
```

**ADO.NET using**
```csharp
using (var conn = new SqlConnection(cs))
using (var cmd = new SqlCommand(sql, conn))
{
    cmd.Parameters.AddWithValue("@id", id);
    conn.Open();
    using (var r = cmd.ExecuteReader())
    {
        while (r.Read()) { /* ... */ }
    }
}
```

**문화권 안전 비교**
```csharp
if (string.Equals(a, b, StringComparison.OrdinalIgnoreCase)) { /* ... */ }
```

---

## 참고: 리뷰어가 특히 보는 위험 신호
- Data Access에서 `using` 누락
- 대규모 루프 내부 `new`/LINQ 체이닝/`ToList()` 반복
- UI 스레드 규칙 위반(Invoke 미사용)
- COM/Interop 해제 누락
- 예외 포착 후 무시/빈 `catch`
- 문자열 비교에서 기본 `==` 남용(문화권 영향)
- 설정 상수/연결 문자열이 코드에 하드코딩
