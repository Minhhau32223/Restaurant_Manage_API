using RestaurantAPI.src.Contract.Table.Request;
using RestaurantAPI.src.Contract.Table.Response;

namespace RestaurantAPI.src.Services.Interfaces;

public interface ITableService {
    Task<TableResponse> Create(CreateTableRequest request);

    Task<List<TableResponse>> GetAll();

    Task<TableResponse?> GetById(long id);

    Task<TableResponse?> Update(long id, UpdateTableRequest request);

    Task<bool> Delete(long id);

    Task<List<TableStatusResponse>> GetStatus();

    Task<bool> Reset(long tableId);

    Task SetOccupied(long tableId);

    Task SetEmpty(long tableId);

    Task<bool> MergeTable(long sourceTableId, long targetTableId);
}