Imports DatosCombos
Imports System.Data
Imports System.IO
Imports System.Net.Mail

Partial Class Usuario
    Inherits System.Web.UI.Page

    Public Shared idExpediente As String
    Dim oExpedientes As New Expedientes
    Dim bActualizar As Boolean

    Protected Sub Page_Load(sender As Object, e As EventArgs) Handles Me.Load
        Dim dtDatos As DataTable

        If Not IsPostBack Then
            'Este es el código de un expediente que nos servirá para probar
            'idExpediente = "1063"
            If Not IsNothing(Request.QueryString("id")) Then
                idExpediente = Request.QueryString("id")
            Else
                idExpediente = 0
            End If

            bActualizar = True
            Cargar_Datos_Combos()

            'Si la variable idExpediente no está vacía, cargaremos los datos del expediente solicitado
            If Not String.IsNullOrEmpty(idExpediente) Then
                'oExpedientes = New Expedientes
                dtDatos = oExpedientes.ObtenerDatos(idExpediente)

                If dtDatos.Rows.Count() > 0 Then
                    Cargar_Datos_Expediente_Controles(dtDatos)
                End If

                Cargar_Grid_Medios_Contacto(idExpediente)
                Cargar_Grid_Aperturas_Cierres(idExpediente)
                Cargar_Grid_Contactos(idExpediente)
                Cargar_Grid_Informes(idExpediente)
                Cargar_Grid_Documentos(idExpediente)
                Cargar_Grid_Actividades(idExpediente)
                Cargar_Grid_Incidencias(idExpediente)
            Else 'En caso de estar vacía es porque queremos añadir un nuevo expediente.
                Limpiar_Controles()
                Cargar_Datos_Combos()
            End If

        End If

    End Sub

    Private Sub Cargar_Datos_Combos()
        Dim oDatosCombos As New DatosCombos

        'Cargar lista de estados del expediente
        Me.cboEstado.DataSource = oDatosCombos.CargarEstados
        Me.cboEstado.DataTextField = "Estado"
        Me.cboEstado.DataValueField = "IdEstado"
        Me.cboEstado.DataBind()

        'Cargar lista de zonas
        Me.cboZona.DataSource = oDatosCombos.CargarZonas
        Me.cboZona.DataTextField = "Zona"
        Me.cboZona.DataValueField = "IdZona"
        Me.cboZona.DataBind()

        'Cargar lista tipo de acceso
        Me.cboAcceso.DataSource = oDatosCombos.CargarTipoAcceso
        Me.cboAcceso.DataTextField = "TipoCaptacion"
        Me.cboAcceso.DataValueField = "IdTipoCaptacion"
        Me.cboAcceso.DataBind()

        'Cargar lista situacion actual
        Me.cboSituacion.DataSource = oDatosCombos.CargarTipoSituacionActual
        Me.cboSituacion.DataTextField = "SituacionActual"
        Me.cboSituacion.DataValueField = "IdSituacionActual"
        Me.cboSituacion.DataBind()

        'Cargar lista educadores
        Me.cboEducador.DataSource = oDatosCombos.CargarEducadores
        Me.cboEducador.DataTextField = "Nombre_Completo"
        Me.cboEducador.DataValueField = "IdEducador"
        Me.cboEducador.DataBind()

        'Cargar lista técnicos PISA
        Me.cboTecnicoPisa.DataSource = oDatosCombos.CargarTecnicos
        Me.cboTecnicoPisa.DataTextField = "Nombre_Completo"
        Me.cboTecnicoPisa.DataValueField = "IdTecnico"
        Me.cboTecnicoPisa.DataBind()

        'Cargar lista destinatarios email. Es el mismo listado que técnicos PISA
        Me.cboDestinatarioEmail.DataSource = oDatosCombos.CargarTecnicos
        Me.cboDestinatarioEmail.DataTextField = "Nombre_Completo"
        Me.cboDestinatarioEmail.DataValueField = "IdTecnico"
        Me.cboDestinatarioEmail.DataBind()

        'Cargar lista sexo
        Me.cboSexo.DataSource = oDatosCombos.CargarSexo
        Me.cboSexo.DataTextField = "Sexo"
        Me.cboSexo.DataValueField = "IdSexo"
        Me.cboSexo.DataBind()

        ''Cargar lista nacionalidades
        Me.cboNacionalidad.DataSource = oDatosCombos.CargarNacionalidades
        Me.cboNacionalidad.DataTextField = "Nacionalidad"
        Me.cboNacionalidad.DataValueField = "IdNacionalidad"
        Me.cboNacionalidad.DataBind()
    End Sub

    Private Sub Cargar_Datos_Expediente_Controles(dtDatos As DataTable)
        Dim Fila As DataRow

        Fila = dtDatos.Rows(0)

        'Cargar datos generales
        Me.lblNombreUsuario.Text = Fila.Item("Nombre_Completo").ToString.Trim 'Este será un campo calculado suma de nombre + apellidos
        Me.txtExpediente.Text = Fila.Item("NumExpCrisol").ToString.Trim
        Me.txtSocialis.Text = Fila.Item("NumExpSocialis").ToString.Trim

        If Not Fila.Item("IdEstado") Is System.DBNull.Value Then
            Me.cboEstado.SelectedValue = Fila.Item("IdEstado")
        Else
            Me.cboEstado.SelectedValue = 1
        End If

        Me.cboZona.SelectedValue = Fila.Item("IdZona")
        Me.cboAcceso.SelectedValue = Fila.Item("IdTipoCaptacion")

        If Not Fila.Item("IdSituacionActual") Is System.DBNull.Value Then
            Me.cboSituacion.SelectedValue = Fila.Item("IdSituacionActual")
        Else
            Me.cboSituacion.SelectedValue = 1
        End If

        'Me.cboTecnicoPisa.SelectedValue = Fila.Item("")
        Me.cboEducador.SelectedValue = Fila.Item("IdEducador")

        'Cargar datos de identificación
        Me.txtNomUsuario.Text = Fila.Item("Nombre").ToString.Trim
        Me.txtApeUsuario.Text = Fila.Item("Apellidos").ToString.Trim

        If Not (Fila.Item("UrlFoto")) Is System.DBNull.Value Then
            Me.FotoMenor.ImageUrl = Fila.Item("UrlFoto").ToString
        Else
            Me.FotoMenor.ImageUrl = "images/no-avatar.jpg"
        End If

        If Not Fila.Item("FechaNacimiento") Is System.DBNull.Value Then
            Me.txtFNacimiento.Text = Format(Fila.Item("FechaNacimiento"), "dd/MM/yyyy")
        Else
            Me.txtFNacimiento.Text = ""
        End If

        Me.txtEdadUsuario.Text = Fila.Item("Edad").ToString.Trim
        Me.cboSexo.SelectedValue = Fila.Item("IdSexo")
        Me.cboNacionalidad.SelectedValue = Fila.Item("IdNacionalidad")

        'Cargar datos de contacto
        Me.txtDomicilio.Text = Fila.Item("Domicilio").ToString.Trim
        Me.txtTelFijo.Text = Fila.Item("TelFijo").ToString.Trim
        Me.txtMovilMenor.Text = Fila.Item("MovilMenor").ToString.Trim
        Me.txtMovilFam.Text = Fila.Item("MovilFamiliar").ToString.Trim


        'Cargar datos de educación
        Me.txtInstituto.Text = Fila.Item("Instituto").ToString
        Me.txtCurso.Text = Fila.Item("Curso").ToString
        Me.txtOtroCentro.Text = Fila.Item("OtroCentro").ToString
        Me.txtOtrosRecursos.Text = Fila.Item("OtrosRecursos").ToString

        'Cargar datos de aperturas y cierres
        If Not Fila("FechaAbierto") Is System.DBNull.Value Then
            Me.txtFAbierto.Text = Format(Fila.Item("FechaAbierto"), "dd/MM/yyyy")
        Else
            Me.txtFAbierto.Text = ""
        End If

        If Not Fila("FechaCerrado") Is System.DBNull.Value Then
            Me.txtFCerrado.Text = Format(Fila.Item("FechaCerrado"), "dd/MM/yyyy")
        Else
            Me.txtFCerrado.Text = ""
        End If

        If Not Fila.Item("FechaReabierto") Is System.DBNull.Value Then
            Me.txtFReabierto.Text = Format(Fila.Item("FechaReabierto"), "dd/MM/yyyy")
        Else
            Me.txtFReabierto.Text = ""
        End If

        If Not Fila.Item("FechaNuevoCierre") Is System.DBNull.Value Then
            Me.txtFNuevoCierre.Text = Format(Fila.Item("FechaNuevoCierre"), "dd/MM/yyyy")
        Else
            Me.txtFNuevoCierre.Text = ""
        End If
        '----
        Me.txtObservaciones.Text = Fila.Item("Observaciones").ToString.Trim
    End Sub

    Protected Sub btnGuardar_Click(sender As Object, e As EventArgs) Handles btnGuardar.Click
        oExpedientes.IdExpediente = Me.txtExpediente.Text.Trim
        oExpedientes.NumSocialis = Me.txtSocialis.Text.Trim
        oExpedientes.IdEstado = Me.cboEstado.SelectedValue
        oExpedientes.IdEducador = Me.cboEducador.SelectedValue
        oExpedientes.IdZona = Me.cboZona.SelectedValue
        oExpedientes.IdTipoCaptacion = Me.cboAcceso.SelectedValue
        oExpedientes.Nombre = Me.txtNomUsuario.Text.Trim
        oExpedientes.Apellidos = Me.txtApeUsuario.Text.Trim
        oExpedientes.UrlFoto = Me.FotoMenor.ImageUrl.ToString
        oExpedientes.IdSexo = Me.cboSexo.SelectedValue
        oExpedientes.IdNacionalidad = Me.cboNacionalidad.SelectedValue
        oExpedientes.FechaNacimiento = CDate(Me.txtFNacimiento.Text.Trim)
        oExpedientes.Domicilio = Me.txtDomicilio.Text.Trim
        oExpedientes.TelFijo = Me.txtTelFijo.Text.Trim
        oExpedientes.MovilMenor = Me.txtMovilMenor.Text.Trim
        oExpedientes.MovilFamiliar = Me.txtMovilFam.Text.Trim
        oExpedientes.Instituto = Me.txtInstituto.Text.Trim
        oExpedientes.OtroCentro = Me.txtOtroCentro.Text.Trim
        oExpedientes.Curso = Me.txtCurso.Text.Trim

        If Not String.IsNullOrEmpty(Me.txtFAbierto.Text.Trim) Then
            oExpedientes.FAbierto = Me.txtFAbierto.Text.Trim
        Else
            oExpedientes.FAbierto = ""
        End If

        If Not String.IsNullOrEmpty(Me.txtFCerrado.Text.Trim) Then
            oExpedientes.FCerrado = Me.txtFCerrado.Text.Trim
        Else
            oExpedientes.FCerrado = ""
        End If

        If Not String.IsNullOrEmpty(Me.txtFReabierto.Text.Trim) Then
            oExpedientes.FReabierto = Me.txtFReabierto.Text.Trim
        Else
            oExpedientes.FReabierto = ""
        End If

        If Not String.IsNullOrEmpty(Me.txtFNuevoCierre.Text.Trim) Then
            oExpedientes.FNuevoCierre = Me.txtFNuevoCierre.Text.Trim
        Else
            oExpedientes.FNuevoCierre = ""
        End If

        oExpedientes.OtrosRecursos = Me.txtOtrosRecursos.Text.Trim
        oExpedientes.IdSituacionActual = Me.cboSituacion.SelectedValue
        oExpedientes.Observaciones = Me.txtObservaciones.Text.Trim

        Me.lblMensaje.Visible = True

        If bActualizar Then

            If oExpedientes.Guardar() Then
                Me.lblMensaje.Text = "Datos guardados"
            Else
                Me.lblMensaje.Text = "No se han guardado los datos"
            End If

        Else

            If oExpedientes.Actualizar() Then
                Me.lblMensaje.Text = "Datos actualizados"
            Else
                Me.lblMensaje.Text = "No se han actualizado los datos"
            End If

        End If

    End Sub

    Private Sub Limpiar_Controles()
        Me.lblNombreUsuario.Text = ""
        Me.txtExpediente.Text = ""
        Me.txtSocialis.Text = ""
        Me.txtNomUsuario.Text = ""
        Me.txtApeUsuario.Text = ""
        Me.txtFNacimiento.Text = ""
        Me.txtEdadUsuario.Text = ""
        Me.txtDomicilio.Text = ""
        Me.txtTelFijo.Text = ""
        Me.txtMovilFam.Text = ""
        Me.txtMovilMenor.Text = ""
        Me.txtInstituto.Text = ""
        Me.txtCurso.Text = ""
        Me.txtOtroCentro.Text = ""
        Me.txtOtrosRecursos.Text = ""
        Me.txtFAbierto.Text = ""
        Me.txtFCerrado.Text = ""
        Me.txtFReabierto.Text = ""
        Me.txtFNuevoCierre.Text = ""
        Me.txtObservaciones.Text = ""
    End Sub

    Private Sub Cargar_Grid_Medios_Contacto(sNumExp As String)
        Dim oDatos As New DatosGrids

        Me.grdMediosContacto.DataSource = oDatos.GridMediosContacto(sNumExp)
        Me.grdMediosContacto.DataBind()

        oDatos = Nothing
    End Sub

    Private Sub Cargar_Grid_Aperturas_Cierres(sNumExp As String)
        Dim oDatos As New DatosGrids

        Me.grdAperturasCierres.DataSource = oDatos.GridAperturasCierres(sNumExp)
        Me.grdAperturasCierres.DataBind()

        oDatos = Nothing
    End Sub

    Private Sub Cargar_Grid_Contactos(sNumExp As String)
        Dim oDatos As New DatosGrids

        Me.grdContactos.DataSource = oDatos.GridContactos(sNumExp)
        Me.grdContactos.DataBind()
    End Sub

    Private Sub Cargar_Grid_Informes(sNumExp As String)
        Dim oDatos As New DatosGrids

        Me.grdInformes.DataSource = oDatos.GridInformes(sNumExp)
        Me.grdInformes.DataBind()
    End Sub

    Public Sub Cargar_Grid_Documentos(sNumExp As String)
        Dim oDatos As New DatosGrids

        Me.grdDocumentos.DataSource = oDatos.GridDocumentos(sNumExp)
        Me.grdDocumentos.DataBind()
    End Sub

    Public Sub Cargar_Grid_Actividades(sNumExp As String)
        Dim oDatos As New DatosGrids

        Me.grdActividades.DataSource = oDatos.GridActividades(sNumExp)
        Me.grdActividades.DataBind()
    End Sub

    Public Sub Cargar_Grid_Incidencias(sNumExp As String)
        Dim oDatos As New DatosGrids

        Me.grdIncidencias.DataSource = oDatos.GridIncidencias(sNumExp)
        Me.grdIncidencias.DataBind()
    End Sub

    Protected Sub lnkMediosContacto_Click(sender As Object, e As EventArgs) Handles lnkMediosContacto.Click
        Response.Redirect("MediosContacto.aspx?id=" + idExpediente)
    End Sub

    Protected Sub grdMediosContacto_RowEditing(sender As Object, e As GridViewEditEventArgs) Handles grdMediosContacto.RowEditing
        'Hemos hecho clic en el botón editar fila
        Response.Redirect("MediosContacto.aspx?id=" + idExpediente + "&idmediocontacto=" + Me.grdMediosContacto.Rows(e.NewEditIndex).Cells(1).Text)

    End Sub

    Protected Sub LinkButton2_Click(sender As Object, e As EventArgs) Handles LinkButton2.Click
        Response.Redirect("AperturasCierres.aspx?id=" + idExpediente)
    End Sub

    Protected Sub grdAperturasCierres_RowEditing(sender As Object, e As GridViewEditEventArgs) Handles grdAperturasCierres.RowEditing
        Response.Redirect("AperturasCierres.aspx?id=" + idExpediente + "&idaperturacierre=" + Me.grdAperturasCierres.Rows(e.NewEditIndex).Cells(1).Text)
    End Sub

    Protected Sub LinkButton3_Click(sender As Object, e As EventArgs) Handles LinkButton3.Click
        Response.Redirect("ExpContacto.aspx?id=" + idExpediente)
    End Sub

    Protected Sub grdContactos_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles grdContactos.PageIndexChanging
        Me.grdContactos.PageIndex = e.NewPageIndex
        Cargar_Grid_Contactos(idExpediente)
    End Sub

    Protected Sub grdContactos_RowEditing(sender As Object, e As GridViewEditEventArgs) Handles grdContactos.RowEditing
        Response.Redirect("ExpContacto.aspx?id=" + idExpediente + "&idcontacto=" + Me.grdContactos.Rows(e.NewEditIndex).Cells(1).Text)
    End Sub

    Protected Sub LinkButton4_Click(sender As Object, e As EventArgs) Handles LinkButton4.Click
        Response.Redirect("ExpInforme.aspx?id=" + idExpediente)
    End Sub

    Protected Sub grdInformes_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles grdInformes.PageIndexChanging
        Me.grdInformes.PageIndex = e.NewPageIndex
        Cargar_Grid_Informes(idExpediente)
    End Sub

    Protected Sub grdInformes_RowEditing(sender As Object, e As GridViewEditEventArgs) Handles grdInformes.RowEditing
        Response.Redirect("ExpInforme.aspx?id=" + idExpediente + "&idinforme=" + Me.grdInformes.Rows(e.NewEditIndex).Cells(1).Text)
    End Sub

    Protected Sub LinkButton1_Click(sender As Object, e As EventArgs) Handles LinkButton1.Click
        Response.Redirect("ExpDocumento.aspx?id=" + idExpediente)
    End Sub

    Protected Sub grdDocumentos_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles grdDocumentos.PageIndexChanging
        Me.grdDocumentos.PageIndex = e.NewPageIndex
        Cargar_Grid_Documentos(idExpediente)
    End Sub

    Protected Sub grdDocumentos_RowEditing(sender As Object, e As GridViewEditEventArgs) Handles grdDocumentos.RowEditing
        Response.Redirect("ExpDocumento.aspx?id=" + idExpediente + "&iddocumento=" + Me.grdDocumentos.Rows(e.NewEditIndex).Cells(1).Text)
    End Sub

    Protected Sub grdIncidencias_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles grdIncidencias.PageIndexChanging
        Me.grdIncidencias.PageIndex = e.NewPageIndex
        Cargar_Grid_Incidencias(idExpediente)
    End Sub

    Protected Sub grdActividades_PageIndexChanging(sender As Object, e As GridViewPageEventArgs) Handles grdActividades.PageIndexChanging
        Me.grdActividades.PageIndex = e.NewPageIndex
        Cargar_Grid_Actividades(idExpediente)
    End Sub

    Protected Sub grdActividades_RowEditing(sender As Object, e As GridViewEditEventArgs) Handles grdActividades.RowEditing
        Response.Redirect("ExpSesionActividad.aspx?id=" + idExpediente + "&idrel=" + Me.grdActividades.Rows(e.NewEditIndex).Cells(1).Text)
    End Sub

    Protected Sub grdIncidencias_RowEditing(sender As Object, e As GridViewEditEventArgs) Handles grdIncidencias.RowEditing
        Response.Redirect("ExpIncidencia.aspx?id=" + idExpediente + "&idincidencia=" + Me.grdIncidencias.Rows(e.NewEditIndex).Cells(1).Text)
    End Sub

    Protected Sub LinkButton5_Click(sender As Object, e As EventArgs) Handles LinkButton5.Click
        Response.Redirect("ExpIncidencia.aspx?id=" + idExpediente)
    End Sub

    Protected Sub LinkButton6_Click(sender As Object, e As EventArgs) Handles LinkButton6.Click
        Response.Redirect("ExpSesionActividad.aspx?id=" + idExpediente)
    End Sub

    Protected Sub btnSubir_Click(sender As Object, e As EventArgs) Handles btnSubir.Click
        Subir_Foto()
    End Sub

    Private Sub Subir_Foto()
        Validar_Archivo()
    End Sub

    Private Sub Validar_Archivo()
        Dim sExt As String = String.Empty
        Dim sNombreArchivo As String = String.Empty

        If Me.FileUpload2.HasFile Then
            sNombreArchivo = Me.FileUpload2.FileName
            sExt = Path.GetExtension(sNombreArchivo)

            If ValidaExtension(sExt) Then
                sNombreArchivo = Me.txtExpediente.Text.Trim + sExt
                Me.FileUpload2.SaveAs(MapPath("~/photos/" + sNombreArchivo))

                Me.FotoMenor.ImageUrl = "photos/" + sNombreArchivo
            End If

        End If

    End Sub

    Private Function ValidaExtension(sExtension As String) As Boolean

        Select Case sExtension
            Case ".jpg", ".jpeg", ".png", ".gif", ".bmp"
                Return True
            Case Else
                Return False
        End Select

    End Function

    Private Sub Enviar_Email(sDestino As String)
        Dim oMsg = New MailMessage
        Dim oSmtp As New SmtpClient
        Dim oTecnicos As New Tecnicos
        Dim sDestinatario As String

        If sDestino.ToUpper = "TRAMA" Then
            oMsg.From = New MailAddress("alguien@micorreo.com", "Servicios Sociales Fuenlabrada")
            oMsg.To.Add("alguien@otrocorreo.com")

        End If

        If sDestino.ToUpper = "SS" Then

            If Me.cboDestinatarioEmail.SelectedValue > 1 Then
                sDestinatario = oTecnicos.VerEmail(CInt(Me.cboDestinatarioEmail.SelectedValue))
            Else
                Exit Sub
            End If


            oMsg.From = New MailAddress("alguien@correo.com", "Trama")
            oMsg.To.Add("yo@correo.com") 'Aquí iria la dirección de email de Servicios Sociales del Ayuntamiento de Fuenlabrada
        End If

        oMsg.Subject = "Derivación de usuario de la zona " + Me.cboZona.SelectedItem.Text
        oMsg.Body = "Derivación hecha por " + Me.cboTecnicoPisa.SelectedItem.Text + vbCrLf + "Usuario: " + Me.lblNombreUsuario.Text

        If sDestino.ToUpper = "SS" Then
            oSmtp.Host = "mail.controlaltsup.com"
            oSmtp.Credentials = New System.Net.NetworkCredential("alguien@correo.com", "Password")
        End If

        If sDestino.ToUpper = "TRAMA" Then
            'Tenemos que sustituirlo por el servidor de correo, el usuario y la contraseña de Servicios Sociales
            oSmtp.Host = "mail.controlaltsup.com"
            oSmtp.Credentials = New System.Net.NetworkCredential("alguien@correo.com", "Password")
        End If
        oSmtp.Send(oMsg)
    End Sub

    Protected Sub btnEmailTrama_Click(sender As Object, e As EventArgs) Handles btnEmailTrama.Click
        Enviar_Email("trama")
    End Sub

    Protected Sub btnEmailServicios_Click(sender As Object, e As EventArgs) Handles btnEmailServicios.Click
        Enviar_Email("ss")
    End Sub

   
    
End Class
