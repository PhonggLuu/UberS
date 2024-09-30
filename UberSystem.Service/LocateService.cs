using OfficeOpenXml;
using UberSystem.Domain.Entities;
using UberSystem.Domain.Interfaces;
using UberSystem.Domain.Interfaces.Services;

namespace UberSystem.Service
{
	public class LocateService : ILocateService
	{
		private readonly IUnitOfWork _unitOfWork;
		public LocateService(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}
		public async Task<Locate?> GetCoordinates(string address, string ward)
		{
			var locateRepository = _unitOfWork.Repository<Locate>();
			var locate = await locateRepository.GetAsync(x => !string.IsNullOrEmpty(x.StartAddress) && !string.IsNullOrEmpty(x.StartWard) &&
																x.StartAddress.ToLower().Trim().Contains(address.ToLower().Trim()) && 
																x.StartWard.ToLower().Trim().Contains(ward.ToLower().Trim()));
			if (locate is null)
				locate = await locateRepository.GetAsync(x => !string.IsNullOrEmpty(x.EndAddress) && !string.IsNullOrEmpty(x.EndWard) &&
																				x.EndAddress.ToLower().Trim().Contains(address.ToLower().Trim()) && 
																				x.EndWard.ToLower().Trim().Contains(ward.ToLower().Trim()));
			return locate;
		}
		public async Task ReadExcelFile(string filePath)
		{
			var locateRepository = _unitOfWork.Repository<Locate>();
			Locate locate;
			try
			{
				ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
				// Đọc file Excel
				FileInfo fileInfo = new FileInfo(filePath);
				using (ExcelPackage package = new ExcelPackage(fileInfo))
				{
					var worksheet = package.Workbook.Worksheets[0];
					int rowCount = worksheet.Dimension.Rows;

					// Đọc dữ liệu từ các cột
					for (int row = 2; row <= rowCount; row++) // Bỏ qua hàng tiêu đề
					{
						var stt = worksheet.Cells[row, 1].Value?.ToString();
						var index = worksheet.Cells[row, 2].Value?.ToString();
						var vehicleId = worksheet.Cells[row, 3].Value?.ToString();
						var pStart = worksheet.Cells[row, 4].Value?.ToString();
						var pTemp = worksheet.Cells[row, 5].Value?.ToString();
						var pEnd = worksheet.Cells[row, 6].Value?.ToString();
						var preRoutes = worksheet.Cells[row, 7].Value?.ToString();
						var freq = worksheet.Cells[row, 8].Value?.ToString();
						var label = worksheet.Cells[row, 9].Value?.ToString();
						var regions = worksheet.Cells[row, 10].Value?.ToString();

						string[] streets = preRoutes.Trim('(', ')').Split(new[] { "', '" }, StringSplitOptions.RemoveEmptyEntries);
						for (int i = 0; i < streets.Length; i++)
						{
							streets[i] = streets[i].Replace("_", " ").Trim('\'');
						}

						string[] wards = regions.Trim('(', ')').Split(new[] { "', '" }, StringSplitOptions.RemoveEmptyEntries);
						for (int i = 0; i < wards.Length; i++)
						{
							wards[i] = wards[i].Replace("_", " ").Trim('\'');
						}

						string[] startLatitudesString = pStart.Trim('(', ')').Split(',');
						double[] startLatitudes = new double[startLatitudesString.Length];
						for (int i = 0; i < startLatitudesString.Length; i++)
						{
							startLatitudes[i] = Convert.ToDouble(startLatitudesString[i].Trim());
						}

						string[] startLongitudesString = pStart.Trim('(', ')').Split(',');
						double[] startLongitudes = new double[startLongitudesString.Length];
						for (int i = 0; i < startLongitudesString.Length; i++)
						{
							startLongitudes[i] = Convert.ToDouble(startLongitudesString[i].Trim());
						}

						string[] tempLatitudesString = pTemp.Trim('(', ')').Split(',');
						double[] tempLatitudes = new double[tempLatitudesString.Length];
						for (int i = 0; i < tempLatitudesString.Length; i++)
						{
							tempLatitudes[i] = Convert.ToDouble(tempLatitudesString[i].Trim());
						}

						string[] tempLongitudesString = pTemp.Trim('(', ')').Split(',');
						double[] tempLongitudes = new double[tempLongitudesString.Length];
						for (int i = 0; i < tempLongitudesString.Length; i++)
						{
							tempLongitudes[i] = Convert.ToDouble(tempLongitudesString[i].Trim());
						}

						string[] endLatitudesString = pEnd.Trim('(', ')').Split(',');
						double[] endLatitudes = new double[endLatitudesString.Length];
						for (int i = 0; i < endLatitudesString.Length; i++)
						{
							endLatitudes[i] = Convert.ToDouble(endLatitudesString[i].Trim());
						}

						string[] endLongitudesString = pEnd.Trim('(', ')').Split(',');
						double[] endLongitudes = new double[endLongitudesString.Length];
						for (int i = 0; i < endLongitudesString.Length; i++)
						{
							endLongitudes[i] = Convert.ToDouble(endLongitudesString[i].Trim());
						}

						locate = new Locate
						{
							Id = row - 1,
							StartLatitude = startLatitudes[0],
							StartLongitude = startLongitudes[1],
							TempLatitude = tempLatitudes[0],
							TempLongitude = tempLongitudes[1],
							EndLatitude = endLatitudes[0],
							EndLongitude = endLongitudes[1],
							StartAddress = streets[0],
							TempAddress = streets[1],
							EndAddress = streets[2],
							StartWard = wards[0],
							TempWard = wards[1],
							EndWard = wards[2]
						};

						await locateRepository.InsertAsync(locate);
					}
				}
			}
			catch (Exception)
			{
				throw;
			}
		}

		public async Task GenerateData(List<Locate> locates)
		{
			await _unitOfWork.BeginTransaction();
			var locateRepository = _unitOfWork.Repository<Locate>();
			_ = locateRepository.InsertRangeAsync(locates);
			await _unitOfWork.CommitTransaction();
		}

	}
}
