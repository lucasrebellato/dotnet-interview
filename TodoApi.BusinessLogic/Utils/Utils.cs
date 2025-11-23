
using TodoApi.IBusinessLogic.Exceptions;

namespace TodoApi.BusinessLogic.Utils;

public static class Utils<T>
    where T : class
{
    public static void CheckForNullValue(T value)
    {
        if (value == null)
        {
            throw new NotFoundException("Recurso no encontrado");
        }
    }
}
