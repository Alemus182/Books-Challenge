namespace Application.Dtos
{
    public record class ErrorDto
    {
        public string Hint { get; set; }
        public string Message { get; set; }
        public string Stack { get; set; }
        
    }
}
