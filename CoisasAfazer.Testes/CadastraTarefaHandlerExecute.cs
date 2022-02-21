using Alura.CoisasAFazer.Core.Commands;
using Alura.CoisasAFazer.Core.Models;
using Alura.CoisasAFazer.Infrastructure;
using Alura.CoisasAFazer.Services.Handlers;
using Castle.Core.Logging;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Linq;
using Xunit;

namespace CoisasAfazer.Testes {
    public class CadastraTarefaHandlerExecute {

        /*
            Dublês para testes
            Dummy Object,
            Fake Object,
            Stubs,
            Mocks,
            Spys        
         */


        [Fact]
        public void DadaTarefaComInfoValidasDeveIncluirNoBD() {

            //Fazes
            //Arrange
            var comando = new CadastraTarefa( "Estudar Xunit", new Categoria( 100, "Estudo" ), new DateTime( 2019, 12, 31 ) );

            var mock = new Mock<ILogger<CadastraTarefaHandler>>();

            var options = new DbContextOptionsBuilder<DbTarefasContext>()
                .UseInMemoryDatabase( "DbTarefasContext" )
                .Options;
            var contexto = new DbTarefasContext( options );
            var repo = new RepositorioTarefa( contexto );

            var handler = new CadastraTarefaHandler( repo, mock.Object );

            //Act

            handler.Execute( comando ); //SUT >> CadastraTarefaHandlerExecute

            //Assert
            var tarefa = repo.ObtemTarefas( t => t.Titulo == "Estudar Xunit" ).FirstOrDefault();
            Assert.NotNull( tarefa );

        }


        [Fact]
        public void QuandoExceptionForLancadaResultadoIsSuccessDeveSerFalse() {

            //arrange
            var comando = new CadastraTarefa( "Estudar Xunit", new Categoria( "Estudo" ), new DateTime( 2019, 12, 31 ) );

            var mensagemDeErroEsperada = "Houve um erro na inclusão de tarefas";

            var mock = new Mock<IRepositorioTarefas>();
            var mockLogger = new Mock<ILogger<CadastraTarefaHandler>>();

            mock.Setup( r => r.IncluirTarefas( It.IsAny<Tarefa[]>() ) )
                .Throws( new Exception( mensagemDeErroEsperada ) );

            var repo = mock.Object;

            var handler = new CadastraTarefaHandler( repo, mockLogger.Object );

            //act
            CommandResult resultado = handler.Execute( comando );

            //assert
            Assert.False( resultado.IsSuccess );
        }

        [Fact]
        public void QuandoExceptionForLancadaDeveLogarAMensagemDaExecao() {

            //arranger
            var mensagemDeErroEsperada = "Houve um erro na inclusão de tarefas";
            var comando = new CadastraTarefa( "Estudar Xunit", new Categoria( "Estudo" ), new DateTime( 2019, 12, 31 ) );
            var mock = new Mock<IRepositorioTarefas>();
            var mockLogger = new Mock<ILogger<CadastraTarefaHandler>>();
            var execaoEsperada = new Exception( mensagemDeErroEsperada );
            var repo = mock.Object;
            var handler = new CadastraTarefaHandler( repo, mockLogger.Object );

            mock.Setup( r => r.IncluirTarefas( It.IsAny<Tarefa[]>() ) )
                .Throws( execaoEsperada );

            //act
            CommandResult resultado = handler.Execute( comando );

            //assert
            mockLogger.Verify( l =>
                l.Log(
                    LogLevel.Error, //Nível de log => LogError
                    It.IsAny<EventId>(), //identificador do evento.
                    It.IsAny<object>(), //objeto que será logado.
                    execaoEsperada, //exeção que será logada
                    It.IsAny<Func<object, Exception, string>>()), //funçao que converte o objeto + exeção em string
                    Times.Once() );

        }

        delegate void CapturaMensagemLog(LogLevel level, EventId eventId, object state, Exception exception, Func<object, Exception, string> function);

        [Fact]
        public void DataTarefaComInfoValidasDeveLogar() {

            //arrange
            var tituloTarefaEsperado = "Usar Moq para aprofundar conhecimento de API";

            var comando = new CadastraTarefa( tituloTarefaEsperado, new Categoria( "Estudo" ), new DateTime( 2019, 12, 31 ) );       

            var mock = new Mock<IRepositorioTarefas>();

            var mockLogger = new Mock<ILogger<CadastraTarefaHandler>>();

            LogLevel levelCapturado = LogLevel.Error;
            string mensagemCapturada = string.Empty;

            CapturaMensagemLog captura = ( level, eventId, state, exception, func ) => {
                levelCapturado = level;
                mensagemCapturada = func( state, exception );
            };

            mockLogger.Setup( l => l.Log(
                    It.IsAny<LogLevel>(), //Nível de log => LogError
                    It.IsAny<EventId>(), //identificador do evento.
                    It.IsAny<object>(), //objeto que será logado.
                    It.IsAny<Exception>(), //exeção que será logada
                    It.IsAny<Func<object, Exception, string>>() //funçao que converte o objeto + exeção em string
                    
                )).Callback( captura );

            var repo = mock.Object;

            var handler = new CadastraTarefaHandler( repo, mockLogger.Object );

            //act
            handler.Execute( comando );

            //assert
            Assert.Equal( LogLevel.Debug, levelCapturado );
            Assert.Contains( tituloTarefaEsperado, mensagemCapturada );

        } 
    }
}
