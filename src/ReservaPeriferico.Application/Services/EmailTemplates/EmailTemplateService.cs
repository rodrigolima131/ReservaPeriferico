using ReservaPeriferico.Application.DTOs;

namespace ReservaPeriferico.Application.Services.EmailTemplates
{
    public interface IEmailTemplateService
    {
        string GerarCorpoEmailSolicitacaoUsuario(ReservaDto reserva, PerifericoDto periferico);
        string GerarCorpoEmailSolicitacaoAdmin(ReservaDto reserva, UsuarioDto usuario, PerifericoDto periferico);
        string GerarCorpoEmailAprovacao(ReservaDto reserva, PerifericoDto periferico, string status, string? motivoRejeicao);
        string GerarCorpoEmailCancelamentoReserva(ReservaDto reserva, PerifericoDto periferico, string status, string? motivoCancelamento);
    }

    public class EmailTemplateService : IEmailTemplateService
    {
        public string GerarCorpoEmailSolicitacaoUsuario(ReservaDto reserva, PerifericoDto periferico)
        {
            return $@"
<!DOCTYPE html>
<html lang=""pt-BR"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Solicitação de Reserva Enviada</title>
    <style>
        body {{
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            line-height: 1.6;
            color: #333;
            max-width: 600px;
            margin: 0 auto;
            padding: 20px;
            background-color: #f5f5f5;
        }}
        .container {{
            background-color: #ffffff;
            border-radius: 10px;
            box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
            overflow: hidden;
        }}
        .header {{
            background: linear-gradient(135deg, #667eea 0%, #764ba2 100%);
            color: white;
            padding: 30px;
            text-align: center;
        }}
        .header h1 {{
            margin: 0;
            font-size: 24px;
            font-weight: 600;
        }}
        .content {{
            padding: 30px;
        }}
        .status-badge {{
            display: inline-block;
            background-color: #e3f2fd;
            color: #1976d2;
            padding: 8px 16px;
            border-radius: 20px;
            font-size: 14px;
            font-weight: 500;
            margin-bottom: 20px;
        }}
        .details-card {{
            background-color: #f8f9fa;
            border-left: 4px solid #667eea;
            padding: 20px;
            margin: 20px 0;
            border-radius: 5px;
        }}
        .details-card h3 {{
            margin-top: 0;
            color: #495057;
            font-size: 18px;
        }}
        .detail-item {{
            margin: 10px 0;
            padding: 8px 0;
            border-bottom: 1px solid #e9ecef;
        }}
        .detail-item:last-child {{
            border-bottom: none;
        }}
        .detail-label {{
            font-weight: 600;
            color: #495057;
            display: inline-block;
            min-width: 120px;
        }}
        .detail-value {{
            color: #212529;
        }}
        .footer {{
            background-color: #f8f9fa;
            padding: 20px;
            text-align: center;
            border-top: 1px solid #e9ecef;
        }}
        .footer p {{
            margin: 0;
            color: #6c757d;
            font-size: 14px;
        }}
        .info-box {{
            background-color: #fff3cd;
            border: 1px solid #ffeaa7;
            color: #856404;
            padding: 15px;
            border-radius: 5px;
            margin: 20px 0;
        }}
        @media (max-width: 600px) {{
            body {{
                padding: 10px;
            }}
            .header, .content, .footer {{
                padding: 20px;
            }}
        }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>✅ Solicitação Enviada</h1>
        </div>
        <div class=""content"">
            <div class=""status-badge"">📋 Aguardando Aprovação</div>
            
            <p>Olá!</p>
            <p>Sua solicitação de reserva foi <strong>registrada com sucesso</strong> e está sendo analisada pela administração da equipe.</p>
            
            <div class=""details-card"">
                <h3>📋 Detalhes da Reserva</h3>
                <div class=""detail-item"">
                    <span class=""detail-label"">Periférico:</span>
                    <span class=""detail-value"">{periferico.Nome}</span>
                </div>
                <div class=""detail-item"">
                    <span class=""detail-label"">Data de Início:</span>
                    <span class=""detail-value"">{reserva.DataInicio:dd/MM/yyyy 'às' HH:mm}</span>
                </div>
                <div class=""detail-item"">
                    <span class=""detail-label"">Data de Fim:</span>
                    <span class=""detail-value"">{reserva.DataFim:dd/MM/yyyy 'às' HH:mm}</span>
                </div>
                <div class=""detail-item"">
                    <span class=""detail-label"">Observações:</span>
                    <span class=""detail-value"">{reserva.Observacoes ?? "Nenhuma observação"}</span>
                </div>
            </div>
            
            <div class=""info-box"">
                <strong>⏳ Próximos Passos:</strong><br>
                Você receberá uma notificação por email assim que sua solicitação for analisada e aprovada ou rejeitada.
            </div>
        </div>
        <div class=""footer"">
            <p><strong>Sistema de Reservas</strong><br>
            Este é um email automático, por favor não responda.</p>
        </div>
    </div>
</body>
</html>";
        }

        public string GerarCorpoEmailSolicitacaoAdmin(ReservaDto reserva, UsuarioDto usuario, PerifericoDto periferico)
        {
            return $@"
<!DOCTYPE html>
<html lang=""pt-BR"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Nova Solicitação de Reserva</title>
    <style>
        body {{
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            line-height: 1.6;
            color: #333;
            max-width: 600px;
            margin: 0 auto;
            padding: 20px;
            background-color: #f5f5f5;
        }}
        .container {{
            background-color: #ffffff;
            border-radius: 10px;
            box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
            overflow: hidden;
        }}
        .header {{
            background: linear-gradient(135deg, #ff6b6b 0%, #ee5a24 100%);
            color: white;
            padding: 30px;
            text-align: center;
        }}
        .header h1 {{
            margin: 0;
            font-size: 24px;
            font-weight: 600;
        }}
        .content {{
            padding: 30px;
        }}
        .status-badge {{
            display: inline-block;
            background-color: #fff3cd;
            color: #856404;
            padding: 8px 16px;
            border-radius: 20px;
            font-size: 14px;
            font-weight: 500;
            margin-bottom: 20px;
        }}
        .details-card {{
            background-color: #f8f9fa;
            border-left: 4px solid #ff6b6b;
            padding: 20px;
            margin: 20px 0;
            border-radius: 5px;
        }}
        .details-card h3 {{
            margin-top: 0;
            color: #495057;
            font-size: 18px;
        }}
        .detail-item {{
            margin: 10px 0;
            padding: 8px 0;
            border-bottom: 1px solid #e9ecef;
        }}
        .detail-item:last-child {{
            border-bottom: none;
        }}
        .detail-label {{
            font-weight: 600;
            color: #495057;
            display: inline-block;
            min-width: 120px;
        }}
        .detail-value {{
            color: #212529;
        }}
        .footer {{
            background-color: #f8f9fa;
            padding: 20px;
            text-align: center;
            border-top: 1px solid #e9ecef;
        }}
        .footer p {{
            margin: 0;
            color: #6c757d;
            font-size: 14px;
        }}
        .action-box {{
            background-color: #e3f2fd;
            border: 1px solid #bbdefb;
            color: #1565c0;
            padding: 15px;
            border-radius: 5px;
            margin: 20px 0;
        }}
        .user-info {{
            background-color: #e8f5e8;
            border-left: 4px solid #4caf50;
            padding: 15px;
            margin: 15px 0;
            border-radius: 5px;
        }}
        @media (max-width: 600px) {{
            body {{
                padding: 10px;
            }}
            .header, .content, .footer {{
                padding: 20px;
            }}
        }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>🔔 Nova Solicitação</h1>
        </div>
        <div class=""content"">
            <div class=""status-badge"">⏳ Aguardando Aprovação</div>
            
            <p>Olá, Administrador!</p>
            <p>Uma nova solicitação de reserva foi enviada e <strong>requer sua aprovação</strong>.</p>
            
            <div class=""user-info"">
                <h4 style=""margin-top: 0; color: #2e7d32;"">👤 Informações do Solicitante</h4>
                <div class=""detail-item"">
                    <span class=""detail-label"">Nome:</span>
                    <span class=""detail-value"">{usuario.Nome}</span>
                </div>
                <div class=""detail-item"">
                    <span class=""detail-label"">Email:</span>
                    <span class=""detail-value"">{usuario.Email}</span>
                </div>
            </div>
            
            <div class=""details-card"">
                <h3>📋 Detalhes da Reserva</h3>
                <div class=""detail-item"">
                    <span class=""detail-label"">Periférico:</span>
                    <span class=""detail-value"">{periferico.Nome}</span>
                </div>
                <div class=""detail-item"">
                    <span class=""detail-label"">Data de Início:</span>
                    <span class=""detail-value"">{reserva.DataInicio:dd/MM/yyyy 'às' HH:mm}</span>
                </div>
                <div class=""detail-item"">
                    <span class=""detail-label"">Data de Fim:</span>
                    <span class=""detail-value"">{reserva.DataFim:dd/MM/yyyy 'às' HH:mm}</span>
                </div>
                <div class=""detail-item"">
                    <span class=""detail-label"">Observações:</span>
                    <span class=""detail-value"">{reserva.Observacoes ?? "Nenhuma observação"}</span>
                </div>
            </div>
            
            <div class=""action-box"">
                <strong>⚡ Ação Necessária:</strong><br>
                Por favor, acesse o sistema para aprovar ou rejeitar esta solicitação de reserva.
            </div>
        </div>
        <div class=""footer"">
            <p><strong>Sistema de Reservas</strong><br>
            Este é um email automático, por favor não responda.</p>
        </div>
    </div>
</body>
</html>";
        }

        public string GerarCorpoEmailAprovacao(ReservaDto reserva, PerifericoDto periferico, string status, string? motivoRejeicao)
        {
            if (status == "Aprovada")
            {
                return $@"
<!DOCTYPE html>
<html lang=""pt-BR"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Reserva Aprovada</title>
    <style>
        body {{
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            line-height: 1.6;
            color: #333;
            max-width: 600px;
            margin: 0 auto;
            padding: 20px;
            background-color: #f5f5f5;
        }}
        .container {{
            background-color: #ffffff;
            border-radius: 10px;
            box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
            overflow: hidden;
        }}
        .header {{
            background: linear-gradient(135deg, #4caf50 0%, #2e7d32 100%);
            color: white;
            padding: 30px;
            text-align: center;
        }}
        .header h1 {{
            margin: 0;
            font-size: 24px;
            font-weight: 600;
        }}
        .content {{
            padding: 30px;
        }}
        .status-badge {{
            display: inline-block;
            background-color: #e8f5e8;
            color: #2e7d32;
            padding: 8px 16px;
            border-radius: 20px;
            font-size: 14px;
            font-weight: 500;
            margin-bottom: 20px;
        }}
        .details-card {{
            background-color: #f8f9fa;
            border-left: 4px solid #4caf50;
            padding: 20px;
            margin: 20px 0;
            border-radius: 5px;
        }}
        .details-card h3 {{
            margin-top: 0;
            color: #495057;
            font-size: 18px;
        }}
        .detail-item {{
            margin: 10px 0;
            padding: 8px 0;
            border-bottom: 1px solid #e9ecef;
        }}
        .detail-item:last-child {{
            border-bottom: none;
        }}
        .detail-label {{
            font-weight: 600;
            color: #495057;
            display: inline-block;
            min-width: 120px;
        }}
        .detail-value {{
            color: #212529;
        }}
        .footer {{
            background-color: #f8f9fa;
            padding: 20px;
            text-align: center;
            border-top: 1px solid #e9ecef;
        }}
        .footer p {{
            margin: 0;
            color: #6c757d;
            font-size: 14px;
        }}
        .success-box {{
            background-color: #e8f5e8;
            border: 1px solid #c8e6c9;
            color: #2e7d32;
            padding: 15px;
            border-radius: 5px;
            margin: 20px 0;
        }}
        @media (max-width: 600px) {{
            body {{
                padding: 10px;
            }}
            .header, .content, .footer {{
                padding: 20px;
            }}
        }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>🎉 Reserva Aprovada!</h1>
        </div>
        <div class=""content"">
            <div class=""status-badge"">✅ Aprovada</div>
            
            <p>Parabéns!</p>
            <p>Sua solicitação de reserva foi <strong>aprovada com sucesso</strong>!</p>
            
            <div class=""success-box"">
                <strong>🎯 Sua reserva está confirmada!</strong><br>
                Você pode retirar o periférico no horário agendado.
            </div>
            
            <div class=""details-card"">
                <h3>📋 Detalhes da Reserva</h3>
                <div class=""detail-item"">
                    <span class=""detail-label"">Periférico:</span>
                    <span class=""detail-value"">{periferico.Nome}</span>
                </div>
                <div class=""detail-item"">
                    <span class=""detail-label"">Data de Início:</span>
                    <span class=""detail-value"">{reserva.DataInicio:dd/MM/yyyy 'às' HH:mm}</span>
                </div>
                <div class=""detail-item"">
                    <span class=""detail-label"">Data de Fim:</span>
                    <span class=""detail-value"">{reserva.DataFim:dd/MM/yyyy 'às' HH:mm}</span>
                </div>
                <div class=""detail-item"">
                    <span class=""detail-label"">Observações:</span>
                    <span class=""detail-value"">{reserva.Observacoes ?? "Nenhuma observação"}</span>
                </div>
            </div>
        </div>
        <div class=""footer"">
            <p><strong>Sistema de Reservas</strong><br>
            Este é um email automático, por favor não responda.</p>
        </div>
    </div>
</body>
</html>";
            }
            else
            {
                return $@"
<!DOCTYPE html>
<html lang=""pt-BR"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Reserva Rejeitada</title>
    <style>
        body {{
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            line-height: 1.6;
            color: #333;
            max-width: 600px;
            margin: 0 auto;
            padding: 20px;
            background-color: #f5f5f5;
        }}
        .container {{
            background-color: #ffffff;
            border-radius: 10px;
            box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
            overflow: hidden;
        }}
        .header {{
            background: linear-gradient(135deg, #f44336 0%, #d32f2f 100%);
            color: white;
            padding: 30px;
            text-align: center;
        }}
        .header h1 {{
            margin: 0;
            font-size: 24px;
            font-weight: 600;
        }}
        .content {{
            padding: 30px;
        }}
        .status-badge {{
            display: inline-block;
            background-color: #ffebee;
            color: #c62828;
            padding: 8px 16px;
            border-radius: 20px;
            font-size: 14px;
            font-weight: 500;
            margin-bottom: 20px;
        }}
        .details-card {{
            background-color: #f8f9fa;
            border-left: 4px solid #f44336;
            padding: 20px;
            margin: 20px 0;
            border-radius: 5px;
        }}
        .details-card h3 {{
            margin-top: 0;
            color: #495057;
            font-size: 18px;
        }}
        .detail-item {{
            margin: 10px 0;
            padding: 8px 0;
            border-bottom: 1px solid #e9ecef;
        }}
        .detail-item:last-child {{
            border-bottom: none;
        }}
        .detail-label {{
            font-weight: 600;
            color: #495057;
            display: inline-block;
            min-width: 120px;
        }}
        .detail-value {{
            color: #212529;
        }}
        .footer {{
            background-color: #f8f9fa;
            padding: 20px;
            text-align: center;
            border-top: 1px solid #e9ecef;
        }}
        .footer p {{
            margin: 0;
            color: #6c757d;
            font-size: 14px;
        }}
        .rejection-box {{
            background-color: #ffebee;
            border: 1px solid #ffcdd2;
            color: #c62828;
            padding: 15px;
            border-radius: 5px;
            margin: 20px 0;
        }}
        .info-box {{
            background-color: #e3f2fd;
            border: 1px solid #bbdefb;
            color: #1565c0;
            padding: 15px;
            border-radius: 5px;
            margin: 20px 0;
        }}
        @media (max-width: 600px) {{
            body {{
                padding: 10px;
            }}
            .header, .content, .footer {{
                padding: 20px;
            }}
        }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>❌ Reserva Rejeitada</h1>
        </div>
        <div class=""content"">
            <div class=""status-badge"">🚫 Rejeitada</div>
            
            <p>Olá!</p>
            <p>Infelizmente sua solicitação de reserva foi <strong>rejeitada</strong>.</p>
            
            <div class=""rejection-box"">
                <strong>📝 Motivo da Rejeição:</strong><br>
                {motivoRejeicao ?? "Não foi fornecido um motivo específico."}
            </div>
            
            <div class=""details-card"">
                <h3>📋 Detalhes da Reserva</h3>
                <div class=""detail-item"">
                    <span class=""detail-label"">Periférico:</span>
                    <span class=""detail-value"">{periferico.Nome}</span>
                </div>
                <div class=""detail-item"">
                    <span class=""detail-label"">Data de Início:</span>
                    <span class=""detail-value"">{reserva.DataInicio:dd/MM/yyyy 'às' HH:mm}</span>
                </div>
                <div class=""detail-item"">
                    <span class=""detail-label"">Data de Fim:</span>
                    <span class=""detail-value"">{reserva.DataFim:dd/MM/yyyy 'às' HH:mm}</span>
                </div>
            </div>
            
            <div class=""info-box"">
                <strong>💬 Precisa de Ajuda?</strong><br>
                Entre em contato com a administração da sua equipe para mais informações ou para solicitar uma nova reserva.
            </div>
        </div>
        <div class=""footer"">
            <p><strong>Sistema de Reservas</strong><br>
            Este é um email automático, por favor não responda.</p>
        </div>
    </div>
</body>
</html>";
            }
        }

        public string GerarCorpoEmailCancelamentoReserva(ReservaDto reserva, PerifericoDto periferico, string status, string? motivoCancelamento)
        {
            return $@"
<!DOCTYPE html>
<html lang=""pt-BR"">
<head>
    <meta charset=""UTF-8"">
    <meta name=""viewport"" content=""width=device-width, initial-scale=1.0"">
    <title>Reserva Cancelada</title>
    <style>
        body {{
            font-family: 'Segoe UI', Tahoma, Geneva, Verdana, sans-serif;
            line-height: 1.6;
            color: #333;
            max-width: 600px;
            margin: 0 auto;
            padding: 20px;
            background-color: #f5f5f5;
        }}
        .container {{
            background-color: #ffffff;
            border-radius: 10px;
            box-shadow: 0 4px 6px rgba(0, 0, 0, 0.1);
            overflow: hidden;
        }}
        .header {{
            background: linear-gradient(135deg, #ff9800 0%, #f57c00 100%);
            color: white;
            padding: 30px;
            text-align: center;
        }}
        .header h1 {{
            margin: 0;
            font-size: 24px;
            font-weight: 600;
        }}
        .content {{
            padding: 30px;
        }}
        .status-badge {{
            display: inline-block;
            background-color: #fff3e0;
            color: #e65100;
            padding: 8px 16px;
            border-radius: 20px;
            font-size: 14px;
            font-weight: 500;
            margin-bottom: 20px;
        }}
        .details-card {{
            background-color: #f8f9fa;
            border-left: 4px solid #ff9800;
            padding: 20px;
            margin: 20px 0;
            border-radius: 5px;
        }}
        .details-card h3 {{
            margin-top: 0;
            color: #495057;
            font-size: 18px;
        }}
        .detail-item {{
            margin: 10px 0;
            padding: 8px 0;
            border-bottom: 1px solid #e9ecef;
        }}
        .detail-item:last-child {{
            border-bottom: none;
        }}
        .detail-label {{
            font-weight: 600;
            color: #495057;
            display: inline-block;
            min-width: 120px;
        }}
        .detail-value {{
            color: #212529;
        }}
        .footer {{
            background-color: #f8f9fa;
            padding: 20px;
            text-align: center;
            border-top: 1px solid #e9ecef;
        }}
        .footer p {{
            margin: 0;
            color: #6c757d;
            font-size: 14px;
        }}
        .cancellation-box {{
            background-color: #fff3e0;
            border: 1px solid #ffcc02;
            color: #e65100;
            padding: 15px;
            border-radius: 5px;
            margin: 20px 0;
        }}
        .info-box {{
            background-color: #e3f2fd;
            border: 1px solid #bbdefb;
            color: #1565c0;
            padding: 15px;
            border-radius: 5px;
            margin: 20px 0;
        }}
        @media (max-width: 600px) {{
            body {{
                padding: 10px;
            }}
            .header, .content, .footer {{
                padding: 20px;
            }}
        }}
    </style>
</head>
<body>
    <div class=""container"">
        <div class=""header"">
            <h1>⚠️ Reserva Cancelada</h1>
        </div>
        <div class=""content"">
            <div class=""status-badge"">🚫 Cancelada</div>
            
            <p>Olá!</p>
            <p>Informamos que a sua reserva foi <strong>cancelada</strong>.</p>
            
            <div class=""cancellation-box"">
                <strong>📝 Motivo do Cancelamento:</strong><br>
                {motivoCancelamento ?? "Não foi fornecido um motivo específico."}
            </div>
            
            <div class=""details-card"">
                <h3>📋 Detalhes da Reserva Cancelada</h3>
                <div class=""detail-item"">
                    <span class=""detail-label"">Periférico:</span>
                    <span class=""detail-value"">{periferico.Nome}</span>
                </div>
                <div class=""detail-item"">
                    <span class=""detail-label"">Data de Início:</span>
                    <span class=""detail-value"">{reserva.DataInicio:dd/MM/yyyy 'às' HH:mm}</span>
                </div>
                <div class=""detail-item"">
                    <span class=""detail-label"">Data de Fim:</span>
                    <span class=""detail-value"">{reserva.DataFim:dd/MM/yyyy 'às' HH:mm}</span>
                </div>
            </div>
            
            <div class=""info-box"">
                <strong>🔄 Nova Reserva?</strong><br>
                Se precisar fazer uma nova reserva, você pode acessar o sistema e solicitar novamente.
            </div>
        </div>
        <div class=""footer"">
            <p><strong>Sistema de Reservas</strong><br>
            Este é um email automático, por favor não responda.</p>
        </div>
    </div>
</body>
</html>";
        }
    }
}

