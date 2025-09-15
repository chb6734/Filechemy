using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.IO;

namespace FileChemy.Implementations
{
	public class ExcelDocumentReader : IDocumentReader
	{

        Application application = new Application();
        Workbook workbook;

        public DocumentType SupportedType => DocumentType.Excel;

		public DocumentContent Read(Stream input)
		{
            Dictionary<int, Worksheet> keyValuePairs = new Dictionary<int, Worksheet>();

            // 1. Stream → 임시파일 저장 
            string tempFile = Path.GetTempFileName();
            
            try
            {
                using (var fileStream = File.Create(tempFile))
                {
                    // .NET Framework 3.5에서는 CopyTo가 없으므로 수동 복사
                    byte[] buffer = new byte[4096];
                    int bytesRead;
                    while ((bytesRead = input.Read(buffer, 0, buffer.Length)) > 0)
                    {
                        fileStream.Write(buffer, 0, bytesRead);
                    }
                }

                // Excel 어플리케이션을 숨김 처리 (사용자에게 보이지 않게)
                application.Visible = false;
                application.ScreenUpdating = false;

                // 2. 임시파일 → Excel 프로세스
                workbook = application.Workbooks.Open(tempFile);

                // 3. Excel 데이터 읽기 - 모든 워크시트 정보 수집
                System.Console.WriteLine($"총 워크시트 수: {workbook.Worksheets.Count}");

                // 변환할 워크시트 찾기
                findSheetToExport(keyValuePairs);


                // 데이터를 바이트 배열로 변환
                byte[] resultData = System.Text.Encoding.UTF8.GetBytes(keyValuePairs[keyValuePairs.Count].ToString());

                // 4. DocumentContent 반환
                return new DocumentContent
                {
                    FilePath = tempFile,
                    Data = resultData, // 실제 Excel 데이터 정보
                    Type = DocumentType.Excel
                };
            }
            catch (System.Exception ex)
            {
                // 오류 발생 시 오류 메시지를 포함한 데이터 반환
                string errorMessage = $"Excel 읽기 오류: {ex.Message}";
                byte[] errorData = System.Text.Encoding.UTF8.GetBytes(errorMessage);
                
                return new DocumentContent
                {
                    FilePath = tempFile,
                    Data = errorData,
                    Type = DocumentType.Excel
                };
            }
            finally
            {
                // 5. 리소스 정리
                if (workbook != null)
                {
                    workbook.Close(false);
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(workbook);
                }
                
                if (application != null)
                {
                    application.Quit();
                    System.Runtime.InteropServices.Marshal.ReleaseComObject(application);
                }
                
                if (File.Exists(tempFile))
                {
                    File.Delete(tempFile);
                }
            }
		}

        /// <summary>
        /// PDF나 이미지로 변환할 워크시트 찾기
        /// Rule: 1. DATA라는 이름이 포함된 시트가 여러개 일 경우
        ///         => 모든 시트를 검사하여 DATA라는 이름이 포함된 시트 중 2행 A열부터 J열까지 검사하여 "Sample ID"가 있는 열 찾기 
        ///             -> 이후 Sample ID가 있는 열의 4행의 값이 있는 마지막 시트의 짝을 반환 
        ///       2. DATA라는 이름이 포함된 시트가 하나일 경우
        ///         => 해당 시트에서 Sample ID가가 있는 열의 마지막 행을 찾아 해당 표의 이름과 같은 시트를 반환
        /// </summary>
        /// <param name="keyValuePairs"></param>
        private void findSheetToExport(Dictionary<int, Worksheet> keyValuePairs)
        {
            int dataSheetCount = 0;
            // 전체 시트를 돌림
            foreach (Worksheet sheet in workbook.Worksheets)
            {
                if (sheet.Name.ToUpper().Contains("DATA"))
                {
                    dataSheetCount++;
                }
                if (dataSheetCount > 1)
                {
                    break;
                }
            }


            if(dataSheetCount > 1)
            {
                // 여러개일 경우
                findSheetWhenMultiple(keyValuePairs);
            }
            else if(dataSheetCount == 1)
            {
                // 하나일 경우
                findSheetWhenSingle(keyValuePairs);
            }
            else
            {
                System.Console.WriteLine("DATA라는 이름이 포함된 시트가 없습니다.");
            }
            
        }

        /// <summary>
        /// DATA 시트가 하나일 때 변환할 워크시트를 찾는 메서드
        /// </summary>
        /// <param name="keyValuePairs">변환할 워크시트를 저장할 딕셔너리 (키: 인덱스, 값: 워크시트)</param>
        /// <remarks>
        /// 처리 과정:
        /// 1. 첫 번째 워크시트에서 2행 A~J열을 검사하여 "Sample ID" 헤더 열을 찾음
        /// 2. Sample ID 열에서 10행 간격으로 점프하면서 마지막 데이터 행을 찾음 (row+3 위치의 셀 값 확인)
        /// 3. 마지막 데이터 행에서 3행 위(lastRow-3)의 A열 값을 테스트 이름으로 사용
        /// 4. 해당 테스트 이름과 일치하는 워크시트를 찾아 keyValuePairs에 저장
        /// </remarks>
        private void findSheetWhenSingle(Dictionary<int, Worksheet> keyValuePairs)
        {
            var ws = (Worksheet)workbook.Worksheets[1];

            int sampleIdColumnIndex = -1;

            //Sample Id 열 찾기
            for (int col = 1; col <= 10; col++) // A열(1)부터 J열(10)까지
            {
                var headerCell = (Range)ws.Cells[2, col];
                string headerValue = headerCell.Value2?.ToString() ?? "";
                if (headerValue.ToUpper().Contains("SAMPLE ID"))
                {
                    sampleIdColumnIndex = col;
                    break;
                }
            }

            int lastRow = -1;

            // Sample ID 열을 찾았다면 sample ID 값이 있는 마지막 행 찾기
            if (sampleIdColumnIndex != -1)
            {
                
                for(int row = 1; row < ws.UsedRange.Rows.Count; row += 10)
                {
                    var cell = (Range)ws.Cells[row+3, sampleIdColumnIndex];
                    string cellValue = cell.Value2?.ToString() ?? "";
                    if (string.IsNullOrEmpty(cellValue))
                    {
                        break;
                    }
                    lastRow = cell.Row;
                }

         

                var sampleIdCell = (Range)ws.Cells[lastRow, sampleIdColumnIndex];
                var lastTestCell = (Range)ws.Cells[lastRow-3, 1];
                string sampleIdValue = sampleIdCell.Value2?.ToString() ?? "";
                string lastTestValue = lastTestCell.Value2?.ToString() ?? "";


                if (sampleIdValue == "")
                {
                    System.Console.WriteLine("Sample ID 값이 없습니다.");
                }
                System.Console.WriteLine($"워크시트 '{lastTestValue}'의 Sample ID: {sampleIdValue}");

                // Sample ID 값과 같은 이름의 시트 찾기
                foreach (Worksheet sheet in workbook.Worksheets)
                {
                    if (sheet.Name.Equals(lastTestValue, StringComparison.OrdinalIgnoreCase))
                    {
                        keyValuePairs[1] = sheet;
                        break;
                    }
                }
            }
            else
            {
                System.Console.WriteLine("Sample ID 열을 찾지 못했습니다.");
            }
        }

        private void findSheetWhenMultiple(Dictionary<int, Worksheet> keyValuePairs)
        {
            for (int i = 1; i <= workbook.Worksheets.Count; i++)
            {
                var ws = (Worksheet)workbook.Worksheets[i];
                if (ws.Name.ToUpper().Contains("DATA"))
                {
                    // 2행 A열부터 J열까지 검사하여 "Sample ID"가 있는 열 찾기
                    int sampleIdColumnIndex = -1;
                    for (int col = 1; col <= 10; col++) // A열(1)부터 J열(10)까지
                    {
                        var headerCell = (Range)ws.Cells[2, col];
                        string headerValue = headerCell.Value2?.ToString() ?? "";
                        if (headerValue.ToUpper().Contains("SAMPLE ID"))
                        {
                            sampleIdColumnIndex = col;
                            break;
                        }
                    }

                    // Sample ID 열을 찾았다면 3행의 값을 가져오기
                    if (sampleIdColumnIndex != -1)
                    {
                        var sampleIdCell = (Range)ws.Cells[4, sampleIdColumnIndex];
                        string sampleIdValue = sampleIdCell.Value2?.ToString() ?? "";
                        if (sampleIdValue == "")
                        {
                            break;
                        }
                        System.Console.WriteLine($"워크시트 '{ws.Name}'의 Sample ID: {sampleIdValue}");
                        keyValuePairs[i] = (Worksheet)workbook.Worksheets[i + 1];
                    }
                }
            }
        }

        public DocumentMetadata GetMetadata(Stream input)
		{
			// TODO: Excel 메타데이터 읽기 구현
			return new DocumentMetadata
			{
				Title = "Excel Document",
				Author = "Unknown",
				PageCount = 1 // Excel의 경우 워크시트 수로 변경 필요
			};
		}
	}

	public class WordDocumentReader : IDocumentReader
	{
		public DocumentType SupportedType => DocumentType.Word;

		public DocumentContent Read(Stream input)
		{
			// TODO: Word 파일 읽기 구현
			// 실제로는 OpenXML SDK 또는 Word Interop을 사용할 예정
			
			var buffer = new byte[input.Length];
			input.Read(buffer, 0, buffer.Length);
			
			return new DocumentContent
			{
				Data = buffer,
				Type = DocumentType.Word
			};
		}

		public DocumentMetadata GetMetadata(Stream input)
		{
			// TODO: Word 메타데이터 읽기 구현
			return new DocumentMetadata
			{
				Title = "Word Document",
				Author = "Unknown",
				PageCount = 1 // 실제 페이지 수로 변경 필요
			};
		}
	}
}