# FileChemy

**문서를 이미지 및 PDF로 변환하는 .NET 라이브러리**

FileChemy는 Excel, Word 문서를 PNG, JPEG, Bitmap, PDF 형식으로 변환할 수 있는 강력하고 확장 가능한 .NET Framework 라이브러리입니다.

---

## 🚀 주요 기능

- **다중 문서 형식 지원**: Excel, Word
- **다양한 출력 형식**: PNG, JPEG, Bitmap, PDF
- **배치 처리 지원**: 여러 파일을 한 번에 변환
- **파이프라인 아키텍처**: 확장 가능한 단계별 처리
- **팩토리 패턴**: 문서 타입별 최적화된 렌더러 제공

---

## 🏗️ 프로젝트 구조

```
FileChemy/
├── src/
│   ├── FileChemy/                    # 메인 라이브러리
│   │   ├── Core/                     # 핵심 인터페이스 및 기본 타입
│   │   │   ├── Primitives.cs         # 기본 열거형, 데이터 클래스
│   │   │   ├── ConversionOptions.cs  # 변환 옵션 설정
│   │   │   ├── IDocumentConverter.cs # 문서 변환기 인터페이스
│   │   │   ├── IDocumentReader.cs    # 문서 읽기 인터페이스
│   │   │   ├── IImageRenderer.cs     # 이미지 렌더링 인터페이스
│   │   │   ├── IPdfRenderer.cs       # PDF 렌더링 인터페이스
│   │   │   └── IRenderer.cs          # 범용 렌더링 인터페이스
│   │   ├── Factories/                # 객체 생성 팩토리
│   │   │   └── RendererFactory.cs    # 렌더러 팩토리 구현
│   │   ├── Implementations/          # 구체적 구현체
│   │   │   ├── DocumentReaders.cs    # 문서 읽기 구현
│   │   │   └── Renderers.cs          # 렌더링 구현
│   │   ├── Pipeline/                 # 처리 파이프라인
│   │   │   ├── IPipelineStep.cs      # 파이프라인 단계 인터페이스
│   │   │   ├── PipelineSteps.cs      # 실제 처리 단계들
│   │   │   └── ConversionPipeline.cs # 전체 변환 파이프라인
│   │   └── FileChemy.cs              # 메인 진입점
│   └── FileChemy.Demo/               # 데모 콘솔 애플리케이션
│       ├── Program.cs                # 데모 프로그램
│       └── Properties/
└── test/
    └── Filechemy.Tests/              # 단위 테스트
```

---

## 🔧 기술 사양

- **.NET Framework**: 3.5, 4.8 지원
- **C# 버전**: 7.3
- **아키텍처 패턴**: Clean Architecture
- **디자인 패턴**: Factory Pattern, Pipeline Pattern, Strategy Pattern

---

## 📦 설치 방법

```bash
# NuGet 패키지 (향후 제공 예정)
Install-Package FileChemy

# 또는 프로젝트 참조 추가
```

---

## 🎯 사용 방법

### 기본 사용법

```csharp
using FileChemy;
using System.IO;

// 기본 옵션으로 Excel을 PNG로 변환
var options = FileChemy.FileChemy.DefaultOptions;
using (var fileStream = File.OpenRead("document.xlsx"))
{
    var converter = new DocumentConverter(pipeline);
    var result = converter.ConvertToImage(fileStream, DocumentType.Excel, new ImageOptions
    {
        ImageFormat = OutputFormat.Png,
        Quality = 95,
        Dpi = 300
    });
    
    if (result.Succeeded)
    {
        // 변환 성공
        foreach (var output in result.Outputs)
        {
            File.WriteAllBytes("output.png", output);
        }
    }
}
```

### PDF 변환

```csharp
var pdfOptions = new PdfOptions
{
    Quality = 90,
    EmbedFonts = true
};

using (var fileStream = File.OpenRead("document.docx"))
{
    var result = converter.ConvertToPdf(fileStream, DocumentType.Word, pdfOptions);
    // 결과 처리...
}
```

### 배치 처리

```csharp
var requests = new List<ConversionRequest>
{
    new ConversionRequest 
    { 
        Input = stream1, 
        DocumentType = DocumentType.Excel, 
        OutputFormat = OutputFormat.Png 
    },
    // 더 많은 요청...
};

var batchResult = converter.ConvertBatch(requests);
```

---

## 🏛️ 아키텍처 설계

### 📁 폴더별 역할

#### 🔧 **Core 폴더** - 핵심 인터페이스 및 기본 타입
**역할**: 라이브러리의 핵심 계약(인터페이스)과 기본 데이터 구조 정의
- 의존성 역전 원칙 적용
- 외부 라이브러리 독립성 보장

#### 🏭 **Factories 폴더** - 객체 생성 팩토리
**역할**: Factory Pattern을 통한 적절한 구현체 생성
- 문서 타입/출력 형식에 따른 렌더러 생성
- 느슨한 결합, 확장성 제공

#### ⚙️ **Implementations 폴더** - 구체적 구현체
**역할**: 인터페이스의 실제 구현 클래스들
- 문서 읽기 및 렌더링 로직 구현
- 새로운 문서 타입이나 출력 형식 쉽게 추가 가능

#### 🔄 **Pipeline 폴더** - 처리 파이프라인
**역할**: Chain of Responsibility 패턴으로 변환 과정을 단계별 처리
- 문서 읽기 → 렌더링 → 결과 반환의 단계적 처리

### 🔄 데이터 흐름

```
ConversionRequest → Pipeline → Factory → Implementation → ConversionResult
```

1. **ConversionRequest**: 변환 요청 정보
2. **Pipeline**: 단계별 처리 조정
3. **Factory**: 적절한 구현체 선택
4. **Implementation**: 실제 변환 작업 수행
5. **ConversionResult**: 변환 결과 반환

---

## 🎮 데모 실행

데모 프로그램을 실행하여 라이브러리의 기본 기능을 확인할 수 있습니다:

```bash
cd src\FileChemy.Demo
dotnet run
```

---

## 🧪 테스트 실행

```bash
cd test\Filechemy.Tests
dotnet test
```

---

## 🔮 향후 계획

### 라이브러리 구현 완성
- [ ] Excel 문서 읽기 구현 (EPPlus/ClosedXML)
- [ ] Word 문서 읽기 구현 (OpenXML SDK)
- [ ] 이미지 렌더링 구현 (System.Drawing/ImageSharp)
- [ ] PDF 렌더링 구현 (iTextSharp/PdfSharp)

### 추가 기능
- [ ] PowerPoint 지원
- [ ] 더 많은 이미지 형식 지원
- [ ] 비동기 처리 지원
- [ ] 진행률 콜백 지원

### 성능 최적화
- [ ] 메모리 사용량 최적화
- [ ] 병렬 처리 지원
- [ ] 캐싱 메커니즘

---

## 📄 라이선스

Copyright © 2025 FileChemy Project

---

## 🤝 기여하기

1. Fork 프로젝트
2. Feature 브랜치 생성 (`git checkout -b feature/AmazingFeature`)
3. 변경사항 커밋 (`git commit -m 'Add some AmazingFeature'`)
4. 브랜치에 Push (`git push origin feature/AmazingFeature`)
5. Pull Request 열기

---

## 📞 지원

문제가 발생하거나 질문이 있으시면 Issues 페이지를 이용해 주세요.
