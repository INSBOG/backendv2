namespace WhotSiv.Api.Services;

public class PasswordService
{
    /// <summary>
    /// Encripta una contraseña usando BCrypt
    /// </summary>
    /// <param name="password">Contraseña en texto plano</param>
    /// <returns>Hash de la contraseña</returns>
    public string HashPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("La contraseña no puede estar vacía", nameof(password));
        }

        return BCrypt.Net.BCrypt.HashPassword(password);
    }

    /// <summary>
    /// Encripta una contraseña con un nivel específico de trabajo
    /// </summary>
    /// <param name="password">Contraseña en texto plano</param>
    /// <param name="workFactor">Factor de trabajo (4-31), por defecto 11. Mayor número = más seguro pero más lento</param>
    /// <returns>Hash de la contraseña</returns>
    public string HashPassword(string password, int workFactor)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            throw new ArgumentException("La contraseña no puede estar vacía", nameof(password));
        }

        if (workFactor < 4 || workFactor > 31)
        {
            throw new ArgumentException("El factor de trabajo debe estar entre 4 y 31", nameof(workFactor));
        }

        return BCrypt.Net.BCrypt.HashPassword(password, workFactor);
    }

    /// <summary>
    /// Verifica si una contraseña coincide con un hash
    /// </summary>
    /// <param name="password">Contraseña en texto plano a verificar</param>
    /// <param name="hash">Hash almacenado de la contraseña</param>
    /// <returns>True si la contraseña es correcta, False si no</returns>
    public bool VerifyPassword(string password, string hash)
    {
        if (string.IsNullOrWhiteSpace(password))
        {
            return false;
        }

        if (string.IsNullOrWhiteSpace(hash))
        {
            return false;
        }

        try
        {
            return BCrypt.Net.BCrypt.Verify(password, hash);
        }
        catch
        {
            // Si el hash es inválido, retornar false
            return false;
        }
    }

    /// <summary>
    /// Verifica si un hash de contraseña necesita ser actualizado (rehashed)
    /// Útil cuando se quiere aumentar el factor de trabajo
    /// </summary>
    /// <param name="hash">Hash de la contraseña</param>
    /// <param name="newWorkFactor">Nuevo factor de trabajo deseado</param>
    /// <returns>True si el hash debe ser actualizado</returns>
    public bool NeedsRehash(string hash, int newWorkFactor = 11)
    {
        if (string.IsNullOrWhiteSpace(hash))
        {
            return true;
        }

        try
        {
            // Extraer el factor de trabajo actual del hash
            var currentWorkFactor = BCrypt.Net.BCrypt.PasswordNeedsRehash(hash, newWorkFactor);
            return currentWorkFactor;
        }
        catch
        {
            return true;
        }
    }
}
