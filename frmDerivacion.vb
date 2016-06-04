Imports DatosCombos
Imports System.IO
Imports cDerivaciones
Imports cInformesSistematicos
Imports cAlertas

Public Class frmDerivacion
    Dim sNombreArchivo As String
    Dim sRutaArchivoOrigen As String
    Dim bNuevaDerivacion As Boolean
    Dim bActivadaValRiesgoSocial As Boolean
    Dim bDatosDerivacionCambiados As Boolean
    Dim DatosUltimaDerivacion As struDatosDevivacion

    Private Sub frmDerivacion_FormClosing(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosingEventArgs) Handles Me.FormClosing
        Dim iRespuesta As Integer

        If bNuevaDerivacion Then
            iRespuesta = MessageBox.Show("¡Atención! Está dando de alta un nueva derivación." + vbCrLf + "Está ud. seguro de querer salir?", "Confirmación salir", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        End If

        If bDatosDerivacionCambiados Then
            iRespuesta = MessageBox.Show("¡Atención! Si ha hecho algún cambio en los datos y sale ahora sin guardar, los cambios se perderán." + vbCrLf + "Está ud. seguro de querer salir?", "Confirmación salir", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
        End If

        If iRespuesta = vbNo Then
            e.Cancel = True
        End If

    End Sub

    Private Sub frmDerivacion_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        My.Application.DoEvents()

        gsEmailProfesionalDerivante = ""
        sNombreArchivo = ""
        sRutaArchivoOrigen = ""
        bNuevaDerivacion = False
        bActivadaValRiesgoSocial = False
        bDatosDerivacionCambiados = False

        Inicilizar_Controles()
        Cargar_Combos_Derivaciones()
        Configurar_Grid_Informes_Derivacion()
        Cargar_Informes_Derivacion()
        Aplicar_Perfiles()

        If glIdDerivacion = 0 Then 'Si es cero, es una nueva derivación.  Esta variable contiene el id de la derivacion seleccionada en la pantalla previa
            bNuevaDerivacion = True

            If DatosUltimaDerivacion.Entidad <> 0 Then

                If MessageBox.Show("¿Desea repetir los datos introducidos de la última derivación?", "Autocompletado de datos", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Yes Then
                    Autocompletar_Datos_Ultima_Derivacion()
                End If

            End If

            'If MessageBox.Show("¿Desea repetir los datos introducidos de la última derivación?", "Autocompletado de datos", MessageBoxButtons.YesNo, MessageBoxIcon.Question) = Windows.Forms.DialogResult.Yes Then

            '    If DatosUltimaDerivacion.Entidad = 0 Then
            '        MessageBox.Show("No hay datos de una última derivación", "Autocompletado de datos", MessageBoxButtons.OK, MessageBoxIcon.Error)
            '        Exit Sub
            '    Else
            '        Autocompletar_Datos_Ultima_Derivacion()
            '    End If

            'End If
        Else 'Si tiene cargado algún valor, tenemos que mostrar los datos de la derivación seleccionada
            bNuevaDerivacion = False

            Cargar_Derivacion(glIdDerivacion, glIdExpediente)
        End If

    End Sub

#Region "FUNCIONES PARA CARGAR COMBOS"

    Private Sub Cargar_Combos_Derivaciones()
        Cargar_Combo_Entidad_Derivante()
        'Cargar_Combo_Organismo_Derivante()
        Cargar_Combo_Organismo_Comunicacion()
        Cargar_Combo_Tipologia_ASI()
        Cargar_Combo_Tipo_Informe_Derivacion()
        Cargar_Combo_Motivos_Devolucion()
    End Sub

    Private Sub Cargar_Combo_Entidad_Derivante()
        Me.cboEntidadDerivante.DataSource = Derivaciones.CargarEntidadesDerivantes
        Me.cboEntidadDerivante.DisplayMember = "NomEntidad"
        Me.cboEntidadDerivante.ValueMember = "IdEntidad"
    End Sub

    Private Sub Cargar_Combo_Organismo_Derivante(ByVal iIdEntidadDerivante As Integer)
        Me.cboOrgDerivante.DataSource = Derivaciones.CargarOrganismosDerivantes(iIdEntidadDerivante)
        Me.cboOrgDerivante.DisplayMember = "NomOrganismoDerivante"
        Me.cboOrgDerivante.ValueMember = "IdOrganismoDerivante"
    End Sub

    Private Sub Cargar_Combo_Organismo_Comunicacion()
        Me.cboOrgCom.DataSource = Derivaciones.CargarOrganismosComunicacion
        Me.cboOrgCom.DisplayMember = "Organismo"
        Me.cboOrgCom.ValueMember = "IdOrganismo"
    End Sub

    Private Sub Cargar_Combo_Tipologia_ASI()
        Me.cboTipoASI.DataSource = Derivaciones.CargarTipologiaASI
        Me.cboTipoASI.DisplayMember = "Tipologia"
        Me.cboTipoASI.ValueMember = "IdTipologiaASI"
    End Sub

    Private Sub Cargar_Combo_Lugar_Denuncia()
        Me.cboLugarDenuncia.DataSource = Derivaciones.CargarLugarDenuncia
        Me.cboLugarDenuncia.DisplayMember = "NomLugarDenuncia"
        Me.cboLugarDenuncia.ValueMember = "IdLugarDenuncia"
    End Sub

    Private Sub Cargar_Combo_Tipo_Informe_Derivacion()
        Me.cboTipoInforme.DataSource = Derivaciones.CargarTipoInformeDerivacion
        Me.cboTipoInforme.DisplayMember = "TipoInforme"
        Me.cboTipoInforme.ValueMember = "IdTipoInformeDer"
    End Sub

    Private Sub Cargar_Combo_Motivos_Devolucion()
        Me.cboMotivoDevProtocolo.DataSource = Derivaciones.CargarMotivosDevolucionProtocolo
        Me.cboMotivoDevProtocolo.DisplayMember = "MotivoDevolucion"
        Me.cboMotivoDevProtocolo.ValueMember = "IdMotivoDevolucion"
    End Sub

#End Region

#Region "RESPUESTAS A EVENTOS"

    Private Sub cboEntidadDerivante_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboEntidadDerivante.SelectedIndexChanged
        Dim iEntidadDerivante As Integer

        If Me.cboEntidadDerivante.SelectedIndex > 0 Then
            iEntidadDerivante = CInt(Me.cboEntidadDerivante.SelectedValue)

            Cargar_Combo_Organismo_Derivante(iEntidadDerivante)

            If iEntidadDerivante = 10 Then
                Me.lblOtraEntidadDerivante.Visible = True
                Me.txtOtraEntidadDerivante.Visible = True
            Else
                Me.lblOtraEntidadDerivante.Visible = False
                Me.txtOtraEntidadDerivante.Visible = False
            End If

        Else
            Me.cboOrgDerivante.DataSource = Nothing
        End If

        'If Me.cboEntidadDerivante.SelectedIndex = 8 Then
        '    Me.lblOtraEntidadDerivante.Visible = True
        '    Me.txtOtraEntidadDerivante.Visible = True
        'Else
        '    Me.lblOtraEntidadDerivante.Visible = False
        '    Me.txtOtraEntidadDerivante.Visible = False
        'End If
    End Sub

    Private Sub cboTipoInforme_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboTipoInforme.SelectedIndexChanged

        If Me.cboTipoInforme.SelectedIndex = 6 Then
            Me.lblOtroTipoInforme.Visible = True
            Me.txtOtroTipoInforme.Visible = True
        Else
            Me.lblOtroTipoInforme.Visible = False
            Me.txtOtroTipoInforme.Visible = False
        End If

    End Sub

    Private Sub cboMotivoDevProtocolo_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboMotivoDevProtocolo.SelectedIndexChanged

        If Me.cboMotivoDevProtocolo.SelectedIndex = 4 Then
            Me.lblOtroMotDevolucion.Visible = True
            Me.txtOtroMotivoDevProtocolo.Visible = True
        Else
            Me.lblOtroMotDevolucion.Visible = False
            Me.txtOtroMotivoDevProtocolo.Visible = False
        End If

    End Sub

    Private Sub dtpFechaAcuseRecibo_CloseUp(ByVal sender As Object, ByVal e As System.EventArgs) Handles dtpFechaAcuseRecibo.CloseUp

        If IsDate(Me.dtpFechaAcuseRecibo.Value) Then
            Me.dtpFechaAcuseRecibo.Format = DateTimePickerFormat.Short
            Me.dtpFechaAcuseRecibo.CustomFormat = ""
        End If

    End Sub

    Private Sub dtpFechaAcuseRecibo_ValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles dtpFechaAcuseRecibo.ValueChanged
        'Para el caso de que queramos deshabilitar una fecha porque nos hayamos equivocado y no debe aparecer
        If Not Me.dtpFechaAcuseRecibo.Checked Then
            Me.dtpFechaAcuseRecibo.Format = DateTimePickerFormat.Custom
            Me.dtpFechaAcuseRecibo.CustomFormat = "  /  /    "
        Else
            Me.dtpFechaAcuseRecibo.Format = DateTimePickerFormat.Short
            Me.dtpFechaAcuseRecibo.CustomFormat = ""
        End If
    End Sub

    Private Sub dtpFechaSalidaDerCIASI_CloseUp(ByVal sender As Object, ByVal e As System.EventArgs) Handles dtpFechaSalidaDerCIASI.CloseUp

        If IsDate(Me.dtpFechaSalidaDerCIASI.Value) Then
            Me.dtpFechaSalidaDerCIASI.Format = DateTimePickerFormat.Short
            Me.dtpFechaSalidaDerCIASI.CustomFormat = ""
        End If

    End Sub

    Private Sub dtpFechaSalidaDerCIASI_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles dtpFechaSalidaDerCIASI.ValueChanged
        'Para el caso de que queramos deshabilitar una fecha porque nos hayamos equivocado y no debe aparecer
        If Not Me.dtpFechaSalidaDerCIASI.Checked Then
            Me.dtpFechaSalidaDerCIASI.Format = DateTimePickerFormat.Custom
            Me.dtpFechaSalidaDerCIASI.CustomFormat = "  /  /    "
        Else
            Me.dtpFechaSalidaDerCIASI.Format = DateTimePickerFormat.Short
            Me.dtpFechaSalidaDerCIASI.CustomFormat = ""
        End If

    End Sub

    Private Sub dtpFecEntradaDerCIASI_CloseUp(ByVal sender As Object, ByVal e As System.EventArgs) Handles dtpFecEntradaDerCIASI.CloseUp

        If IsDate(Me.dtpFecEntradaDerCIASI.Value) Then
            Me.dtpFecEntradaDerCIASI.Format = DateTimePickerFormat.Short
            Me.dtpFecEntradaDerCIASI.CustomFormat = ""
        End If

    End Sub

    Private Sub dtpFecEntradaDerCIASI_ValueChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles dtpFecEntradaDerCIASI.ValueChanged

        If Not Me.dtpFecEntradaDerCIASI.Checked Then
            Me.dtpFecEntradaDerCIASI.Format = DateTimePickerFormat.Custom
            Me.dtpFecEntradaDerCIASI.CustomFormat = "  /  /    "
        Else
            Me.dtpFecEntradaDerCIASI.Format = DateTimePickerFormat.Short
            Me.dtpFecEntradaDerCIASI.CustomFormat = ""
        End If

    End Sub

    Private Sub dtpFDerConsejeria_CloseUp(ByVal sender As Object, ByVal e As System.EventArgs) Handles dtpFDerConsejeria.CloseUp

        If IsDate(Me.dtpFDerConsejeria.Value) Then
            Me.dtpFDerConsejeria.Format = DateTimePickerFormat.Short
            Me.dtpFDerConsejeria.CustomFormat = ""
        End If

    End Sub

    Private Sub dtpFDerConsejeria_ValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles dtpFDerConsejeria.ValueChanged

        If Not Me.dtpFDerConsejeria.Checked Then
            Me.dtpFDerConsejeria.Format = DateTimePickerFormat.Custom
            Me.dtpFDerConsejeria.CustomFormat = "  /  /    "
        Else
            Me.dtpFDerConsejeria.Format = DateTimePickerFormat.Short
            Me.dtpFDerConsejeria.CustomFormat = ""
        End If

    End Sub

    Private Sub dtpFEnvioFiscalia_CloseUp(ByVal sender As Object, ByVal e As System.EventArgs) Handles dtpFEnvioFiscalia.CloseUp

        If IsDate(Me.dtpFEnvioFiscalia.Value) Then
            Me.dtpFEnvioFiscalia.Format = DateTimePickerFormat.Short
            Me.dtpFEnvioFiscalia.CustomFormat = ""
        End If

    End Sub

    Private Sub dtpFEnvioFiscalia_ValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles dtpFEnvioFiscalia.ValueChanged

        If Not Me.dtpFEnvioFiscalia.Checked Then
            Me.dtpFEnvioFiscalia.Format = DateTimePickerFormat.Custom
            Me.dtpFEnvioFiscalia.CustomFormat = "  /  /    "
        Else
            Me.dtpFEnvioFiscalia.Format = DateTimePickerFormat.Short
            Me.dtpFEnvioFiscalia.CustomFormat = ""
        End If

    End Sub

    Private Sub dtpFAtencionUrg_CloseUp(ByVal sender As Object, ByVal e As System.EventArgs) Handles dtpFAtencionUrg.CloseUp

        If IsDate(Me.dtpFAtencionUrg.Value) Then
            Me.dtpFAtencionUrg.Format = DateTimePickerFormat.Short
            Me.dtpFAtencionUrg.CustomFormat = ""
        End If

    End Sub

    Private Sub dtpFAtencionUrg_ValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles dtpFAtencionUrg.ValueChanged

        If Not Me.dtpFAtencionUrg.Checked Then
            Me.dtpFAtencionUrg.Format = DateTimePickerFormat.Custom
            Me.dtpFAtencionUrg.CustomFormat = "  /  /    "
        Else
            Me.dtpFAtencionUrg.Format = DateTimePickerFormat.Short
            Me.dtpFAtencionUrg.CustomFormat = ""
        End If

    End Sub

    Private Sub btnSelArchivo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSelArchivo.Click
        Seleccionar_Archivo()
    End Sub

    Private Sub btnSubirArchivo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnSubirArchivo.Click
        Subir_Archivo()
    End Sub

    Private Sub picGuardar_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles picGuardar.Click

        If bNuevaDerivacion Then
            Guardar_Nueva_Derivacion()
            bNuevaDerivacion = False
            Me.Close()
        Else

            If bDatosDerivacionCambiados Then
                Actualizar_Derivacion()
            End If

        End If

    End Sub

    Private Sub btnEnviarAcuseRecibo_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles btnEnviarAcuseRecibo.Click
        Me.dtpFechaAcuseRecibo.Value = Now
        Me.dtpFechaAcuseRecibo.Checked = True

        Enviar_Acuse_Recibo()
    End Sub

    Private Sub grdInformesDerivacion_CellDoubleClick(ByVal sender As Object, ByVal e As System.Windows.Forms.DataGridViewCellEventArgs) Handles grdInformesDerivacion.CellDoubleClick
        Dim sArchivo, sTipoInforme As String

        If e.RowIndex >= 0 Then
            gsArchivoPdf = ""
            sArchivo = Me.grdInformesDerivacion.Rows(e.RowIndex).Cells("colArchivo").Value.ToString
            sTipoInforme = Me.grdInformesDerivacion.Rows(e.RowIndex).Cells("colTipoInforme").Value.ToString

            Select Case sTipoInforme
                Case "Atestado"
                    gsArchivoPdf = gsDirFinalDerAtestados + sArchivo
                Case "Médico"
                    gsArchivoPdf = gsDirFinalDerMedicos + sArchivo
                Case "Social"
                    gsArchivoPdf = gsDirFinalDerSociales + sArchivo
                Case "Educativo"
                    gsArchivoPdf = gsDirFinalDerEducativos + sArchivo
                Case "Diligencias"
                    gsArchivoPdf = gsDirFinalDerDiligencias + sArchivo
                Case "Derivación"
                    gsArchivoPdf = gsDirFinalDerDerivaciones + sArchivo
                Case "Denuncia"
                    gsArchivoPdf = gsDirFinalDerDenuncias + sArchivo
                Case "Otros"
                    gsArchivoPdf = gsDirFinalDerOtros + sArchivo
            End Select

            frmVisorPDF.ShowDialog()
        End If

    End Sub

    Private Sub picGuardar_MouseEnter(ByVal sender As Object, ByVal e As System.EventArgs) Handles picGuardar.MouseEnter
        Me.lblGuardar.ForeColor = Color.Orange
    End Sub

    Private Sub picGuardar_MouseLeave(ByVal sender As Object, ByVal e As System.EventArgs) Handles picGuardar.MouseLeave
        Me.lblGuardar.ForeColor = Color.SlateBlue
    End Sub

    Private Sub cboOrgDerivante_SelectedIndexChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles cboOrgDerivante.SelectedIndexChanged
        'Cuando seleccionamos un organismo derivante mostramos los datos relativos a ese organismo
        Dim iOrganismo As Integer
        Dim dtDatosOrganismo As DataTable

        If Me.cboOrgDerivante.SelectedIndex > 0 Then
            iOrganismo = CInt(Me.cboOrgDerivante.SelectedValue)
            dtDatosOrganismo = Derivacion.CargarOrganismoDerivante(iOrganismo)

            If Not dtDatosOrganismo Is Nothing Then
                Cargar_Datos_Organismo_En_Controles(dtDatosOrganismo)
            End If

        End If

    End Sub

    Private Sub cboOrgCom_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboOrgCom.SelectedIndexChanged
        Dim iOrganismo As Integer

        If Me.cboOrgCom.SelectedIndex > 0 Then
            iOrganismo = CInt(Me.cboOrgCom.SelectedValue)

            If iOrganismo > 2 Then
                Me.txtOtrosOrgCom.Visible = True
                Me.txtOtrosOrgCom.Text = ""
            Else
                Me.txtOtrosOrgCom.Visible = False
            End If

        End If

    End Sub

    Private Sub rbtnDenunciado_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rbtnDenunciado.CheckedChanged

        If Me.rbtnDenunciado.Checked Then
            Me.cboLugarDenuncia.Visible = True

            Cargar_Combo_Lugar_Denuncia()
        Else
            Me.cboLugarDenuncia.Visible = False
            Me.grDenunciadoPolicia.Visible = False
            Me.cboLugarDenuncia.DataSource = Nothing
        End If

    End Sub

    Private Sub cboLugarDenuncia_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles cboLugarDenuncia.SelectedIndexChanged
        Dim iLugar As Integer

        If Me.cboLugarDenuncia.SelectedIndex > 0 Then
            iLugar = CInt(Me.cboLugarDenuncia.SelectedValue)

            If iLugar = 2 Then
                Me.grDenunciadoPolicia.Visible = True
            Else
                Me.grDenunciadoPolicia.Visible = False
            End If

        End If

    End Sub

    Private Sub rbtnComSi_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rbtnComSi.CheckedChanged

        If Me.rbtnComSi.Checked Then
            Me.lblOrgCom.Visible = True
            Me.cboOrgCom.Visible = True

            If CInt(Me.cboOrgCom.SelectedValue) > 2 Then
                Me.txtOtrosOrgCom.Visible = True
            End If

        Else
            Me.lblOrgCom.Visible = False
            Me.cboOrgCom.Visible = False
            Me.txtOtrosOrgCom.Visible = False
        End If

    End Sub

    Private Sub rbtnUrgenteSi_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rbtnUrgenteSi.CheckedChanged

        If Me.rbtnUrgenteSi.Checked Then
            Me.dtpFAtencionUrg.Visible = True
            Me.lblFAtencion.Visible = True
        Else
            Me.dtpFAtencionUrg.Visible = False
            Me.lblFAtencion.Visible = False
        End If

    End Sub

    Private Sub rbtnConsejeriaSi_CheckedChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles rbtnConsejeriaSi.CheckedChanged

        If Me.rbtnConsejeriaSi.Checked Then
            Me.dtpFDerConsejeria.Visible = True
            Me.lblFConsejeria.Visible = True
        Else
            Me.dtpFDerConsejeria.Visible = False
            Me.lblFConsejeria.Visible = False
        End If

    End Sub

    Private Sub dtpFSalidaDev_CloseUp(ByVal sender As Object, ByVal e As System.EventArgs) Handles dtpFSalidaDev.CloseUp

        If IsDate(Me.dtpFSalidaDev.Value) Then
            Me.dtpFSalidaDev.Format = DateTimePickerFormat.Short
            Me.dtpFSalidaDev.CustomFormat = ""
        End If

    End Sub

    Private Sub dtpFSalidaDev_ValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles dtpFSalidaDev.ValueChanged

        If Not Me.dtpFSalidaDev.Checked Then
            Me.dtpFSalidaDev.Format = DateTimePickerFormat.Custom
            Me.dtpFSalidaDev.CustomFormat = "  /  /    "
        Else
            Me.dtpFSalidaDev.Format = DateTimePickerFormat.Short
            Me.dtpFSalidaDev.CustomFormat = ""
        End If

    End Sub

    Private Sub dtpFEntradaSub_CloseUp(ByVal sender As Object, ByVal e As System.EventArgs) Handles dtpFEntradaSub.CloseUp

        If IsDate(Me.dtpFEntradaSub.Value) Then
            Me.dtpFEntradaSub.Format = DateTimePickerFormat.Short
            Me.dtpFEntradaSub.CustomFormat = ""
        End If

    End Sub

    Private Sub dtpFEntradaSub_ValueChanged(ByVal sender As Object, ByVal e As System.EventArgs) Handles dtpFEntradaSub.ValueChanged

        If Not Me.dtpFEntradaSub.Checked Then
            Me.dtpFEntradaSub.Format = DateTimePickerFormat.Custom
            Me.dtpFEntradaSub.CustomFormat = "  /  /    "
        Else
            Me.dtpFEntradaSub.Format = DateTimePickerFormat.Short
            Me.dtpFEntradaSub.CustomFormat = ""
        End If

    End Sub

#End Region

#Region "FUNCIONES"

    Private Sub Inicilizar_Controles()
        Me.dtpFechaEntradaIMFM.Value = Now
        Me.rbtnRiesgoNo.Checked = True
        Me.lblOtraEntidadDerivante.Visible = False
        Me.txtOtraEntidadDerivante.Visible = False
        Me.txtNomOrganismo.Text = ""
        Me.txtDirOrgDerivante.Text = ""
        Me.txtTelOrgDerivante.Text = ""
        Me.txtFaxOrgDerivante.Text = ""
        Me.txtEmailOrganismo.Text = ""
        Me.txtNomProfDerivante.Text = ""
        Me.txtCargoProfDerivante.Text = ""
        Me.txtEmailProfDerivante.Text = ""
        Me.txtTelProfDerivante.Text = ""
        Me.txtFaxProfDerivante.Text = ""
        Me.rbtnComNo.Checked = True
        Me.cboOrgCom.Visible = False
        Me.lblOrgCom.Visible = False
        Me.txtOtrosOrgCom.Text = ""
        Me.txtOtrosOrgCom.Visible = False
        Me.rbtnComNo.Checked = True
        Me.rbtnSinDenunciar.Checked = True
        Me.grDenunciadoPolicia.Visible = False
        Me.txtAtestado.Text = ""
        Me.txtDepPolicial.Text = ""
        Me.lblOtroTipoInforme.Visible = False
        Me.txtOtroTipoInforme.Visible = False
        Me.lblFAtencion.Visible = False
        Me.dtpFAtencionUrg.Visible = False
        Me.dtpFAtencionUrg.Format = DateTimePickerFormat.Custom
        Me.dtpFAtencionUrg.CustomFormat = "  /  /    "
        Me.dtpFAtencionUrg.Checked = False
        Me.dtpFechaAcuseRecibo.Format = DateTimePickerFormat.Custom
        Me.dtpFechaAcuseRecibo.CustomFormat = "  /  /    "
        Me.dtpFechaAcuseRecibo.Checked = False
        Me.lblOtroMotDevolucion.Visible = False
        Me.txtOtroMotivoDevProtocolo.Visible = False
        Me.dtpFSalidaDev.Format = DateTimePickerFormat.Custom
        Me.dtpFSalidaDev.CustomFormat = "  /  /    "
        Me.dtpFSalidaDev.Checked = False
        Me.dtpFEntradaSub.Format = DateTimePickerFormat.Custom
        Me.dtpFEntradaSub.CustomFormat = "  /  /    "
        Me.dtpFEntradaSub.Checked = False
        Me.dtpFechaSalidaDerCIASI.Format = DateTimePickerFormat.Custom
        Me.dtpFechaSalidaDerCIASI.CustomFormat = "  /  /    "
        Me.dtpFechaSalidaDerCIASI.Checked = False
        Me.dtpFEnvioFiscalia.Format = DateTimePickerFormat.Custom
        Me.dtpFEnvioFiscalia.CustomFormat = "  /  /    "
        Me.dtpFEnvioFiscalia.Checked = False
        Me.rbtnConsejeriaNo.Checked = True
        Me.lblFConsejeria.Visible = False
        Me.dtpFDerConsejeria.Format = DateTimePickerFormat.Custom
        Me.dtpFDerConsejeria.CustomFormat = "  /  /    "
        Me.dtpFDerConsejeria.Checked = False
        Me.dtpFDerConsejeria.Visible = False
        Me.dtpFecEntradaDerCIASI.Format = DateTimePickerFormat.Custom
        Me.dtpFecEntradaDerCIASI.CustomFormat = "  /  /    "
        Me.dtpFecEntradaDerCIASI.Checked = False
        Me.txtArchivoSeleccionado.Text = ""
    End Sub

    Private Function AdjuntarInformeDerivacion(ByVal iIdDerivacion As Integer, ByVal iTipoInforme As Integer, ByVal sOtroTipoInforme As String, ByVal sNombreFinalArchivo As String) As Boolean

        If Derivacion.AdjuntarInforme(iIdDerivacion, iTipoInforme, sOtroTipoInforme, sNombreFinalArchivo) Then
            Return True
        Else
            Return False
        End If

    End Function

    Private Sub Cargar_Informes_Derivacion()
        Dim dtInformesDerivacion As DataTable

        dtInformesDerivacion = Derivacion.CargarInformesDerivacion(CInt(glIdDerivacion))

        If dtInformesDerivacion.Rows.Count > 0 Then

            Me.grdInformesDerivacion.DataSource = dtInformesDerivacion
        Else
            Me.grdInformesDerivacion.DataSource = Nothing
        End If

    End Sub

    Private Sub Configurar_Grid_Informes_Derivacion()
        Me.grdInformesDerivacion.DataSource = ""
        Me.grdInformesDerivacion.Columns.Clear()
        'Añadimos las columnas que se van a mostrar
        Me.grdInformesDerivacion.Columns.Add("colId", "Id")
        Me.grdInformesDerivacion.Columns.Add("colTipoInforme", "Tipo informe")
        Me.grdInformesDerivacion.Columns.Add("colArchivo", "Archivo")
        'Enlace de las columnas con los datos
        Me.grdInformesDerivacion.Columns("colId").DataPropertyName = "IdInforme"
        Me.grdInformesDerivacion.Columns("colTipoInforme").DataPropertyName = "TipoInforme"
        Me.grdInformesDerivacion.Columns("colArchivo").DataPropertyName = "NombreArchivo"
        'Propiedades de las columnas
        Me.grdInformesDerivacion.Columns("colId").Width = 25
        Me.grdInformesDerivacion.Columns("colTipoInforme").Width = 150
        Me.grdInformesDerivacion.Columns("colArchivo").Width = 100
        'Columnas que ocultaremos después
        Me.grdInformesDerivacion.Columns("colId").Visible = False
        Me.grdInformesDerivacion.Columns("colTipoInforme").Visible = True
        Me.grdInformesDerivacion.Columns("colArchivo").Visible = True
    End Sub

    Private Sub Guardar_Nueva_Derivacion()
        Dim dtmFEntradaIMFM, dtmFSolAtencion, dtmFAcuseRecibo, dtmFSalidaCIASI, dtmFSalidaFiscalia, dtmFDerConsejeria, dtmFSalidaDev, dtmFEntradaSub As Date
        Dim bFamiliaRiesgo As Boolean = False
        Dim bComunicacion As Boolean = False
        Dim bConvivencia As Boolean = False
        Dim bDenunciado As Boolean = False
        Dim bAtencionUrgente As Boolean = False
        Dim bEnvioConsejeria As Boolean = False
        Dim iIdEntidadDer, iIdOrganismoDer, iIdTipoInforme, iIdMotivoDevolucion, iIdOrgCom, iIdTipologiaASI, iIdLugarDenuncia As Integer
        Dim sOtraEntidadDer, sNomOrganismo, sDirOrganismoDer, sTelOrganismoDer, sFaxOrganismoDer, sEmailOrganismoDer, sNomProfesional, sCargoProfesional, sEmailProfesional, sTelProfesional, sFaxProfesional, sOtroTipoInforme, sOtroMotivoDevolucion, sSQL, sOtrosOrgCom, sAtestado, sDependenciaPolicial As String

        dtmFEntradaIMFM = Me.dtpFechaEntradaIMFM.Value.Date
       
        If Me.rbtnRiesgoSI.Checked Then
            bFamiliaRiesgo = True
        Else
            bFamiliaRiesgo = False
        End If

        iIdEntidadDer = CInt(Me.cboEntidadDerivante.SelectedValue)
        DatosUltimaDerivacion.Entidad = iIdEntidadDer
        sOtraEntidadDer = Me.txtOtraEntidadDerivante.Text.Trim
        DatosUltimaDerivacion.OtraEntidad = sOtraEntidadDer
        iIdOrganismoDer = CInt(Me.cboOrgDerivante.SelectedValue)
        DatosUltimaDerivacion.Organismo = iIdOrganismoDer
        sNomOrganismo = Me.txtNomOrganismo.Text.Trim
        DatosUltimaDerivacion.NomOrganismo = sNomOrganismo
        sDirOrganismoDer = Me.txtDirOrgDerivante.Text.Trim
        DatosUltimaDerivacion.DirOrganismo = sDirOrganismoDer
        sTelOrganismoDer = Me.txtTelOrgDerivante.Text.Trim
        DatosUltimaDerivacion.TelOrganismo = sTelOrganismoDer
        sFaxOrganismoDer = Me.txtFaxOrgDerivante.Text.Trim
        DatosUltimaDerivacion.FaxOrganismo = sFaxOrganismoDer
        sEmailOrganismoDer = Me.txtEmailOrganismo.Text.Trim
        DatosUltimaDerivacion.EmailOrganismo = sEmailOrganismoDer
        sNomProfesional = Me.txtNomProfDerivante.Text.Trim
        DatosUltimaDerivacion.NomProfesional = sNomProfesional
        sCargoProfesional = Me.txtCargoProfDerivante.Text.Trim
        DatosUltimaDerivacion.CargoProfesional = sCargoProfesional
        sEmailProfesional = Me.txtEmailProfDerivante.Text.Trim
        DatosUltimaDerivacion.EmailProfesional = sEmailProfesional
        sTelProfesional = Me.txtTelProfDerivante.Text.Trim
        DatosUltimaDerivacion.TelProfesional = sTelProfesional
        sFaxProfesional = (Me.txtFaxProfDerivante.Text.Trim)
        DatosUltimaDerivacion.FaxProfesional = sFaxProfesional

        If Me.rbtnComSi.Checked Then
            bComunicacion = True
        Else
            bComunicacion = False
        End If

        DatosUltimaDerivacion.ComOtroOrganismo = bComunicacion
        iIdOrgCom = CInt(Me.cboOrgCom.SelectedValue)
        DatosUltimaDerivacion.ComOrganismo = iIdOrgCom
        sOtrosOrgCom = Me.txtOtrosOrgCom.Text.Trim
        DatosUltimaDerivacion.ComNomOtroOrganismo = sOtrosOrgCom
        iIdTipologiaASI = CInt(Me.cboTipoASI.SelectedValue)

        If Me.rbtnConvSi.Checked Then
            bConvivencia = True
        Else
            bConvivencia = False
        End If

        If Me.rbtnDenunciado.Checked Then
            bDenunciado = True
        Else
            bDenunciado = False
        End If

        iIdLugarDenuncia = CInt(Me.cboLugarDenuncia.SelectedValue)
        sAtestado = Me.txtAtestado.Text.Trim
        sDependenciaPolicial = Me.txtDepPolicial.Text.Trim

        iIdTipoInforme = CInt(Me.cboTipoInforme.SelectedValue)
        sOtroTipoInforme = Me.txtOtroTipoInforme.Text.Trim

        If Me.rbtnUrgenteSi.Checked Then
            bAtencionUrgente = True

            If Me.dtpFAtencionUrg.Checked Then
                dtmFSolAtencion = Me.dtpFAtencionUrg.Value.Date
            Else
                dtmFSolAtencion = Nothing
            End If

        Else
            bAtencionUrgente = False
        End If

        If Me.dtpFechaAcuseRecibo.Checked Then
            dtmFAcuseRecibo = Me.dtpFechaAcuseRecibo.Value
        Else
            dtmFAcuseRecibo = Nothing
        End If

        iIdMotivoDevolucion = CInt(Me.cboMotivoDevProtocolo.SelectedValue)
        sOtroMotivoDevolucion = Me.txtOtroMotivoDevProtocolo.Text.Trim

        If Me.dtpFSalidaDev.Checked Then
            dtmFSalidaDev = Me.dtpFSalidaDev.Value.Date
        Else
            dtmFSalidaDev = Nothing
        End If

        If Me.dtpFEntradaSub.Checked Then
            dtmFEntradaSub = Me.dtpFEntradaSub.Value.Date
        Else
            dtmFEntradaSub = Nothing
        End If

        If Me.dtpFEnvioFiscalia.Checked Then
            dtmFSalidaFiscalia = Me.dtpFEnvioFiscalia.Value.Date
        Else
            dtmFSalidaFiscalia = Nothing
        End If

        If Me.rbtnConsejeriaSi.Checked Then
            bEnvioConsejeria = True

            If Me.dtpFDerConsejeria.Checked Then
                dtmFDerConsejeria = Me.dtpFDerConsejeria.Value.Date
            Else
                dtmFDerConsejeria = Nothing
            End If

        Else
            bEnvioConsejeria = False
        End If

        sSQL = "INSERT INTO Derivaciones (IdExpediente, FEntradaIMFM, RiesgoFamilia, IdEntidadDerivante, OtraEntidadDerivante, IdOrganismoDerivante, NomOrganismo, DirOrganismo, TelOrganismo, FaxOrganismo, EmailOrganismo, ProDerivante, CargoProDerivante, TelProDerivante, FaxProDerivante, EmailProDerivante, ComSituacion, IdOrgCom, OtrosOrgCom, IdTipologiaASi, Convivencia, Denunciado, IdLugarDenuncia, Atestado, DependenciaPolicial, IdMotivoDevolucion, OtroMotivoDevolucion, FSalidaDevolucion, FEntradaSubsanacion, AtencionUrgente, FAtencionUrgente, FEnvioFiscalia, EnvioConsejeria, FEnvioConsejeria, FSalidaDerCIASI, CreadoPor, FCreadoPor, ModificadoPor, FModificadoPor) VALUES(" + glIdExpediente.ToString + ", '" + Format(dtmFEntradaIMFM, "dd/MM/yyyy") + "', " + CInt(bFamiliaRiesgo).ToString + "," + iIdEntidadDer.ToString + ", '" + sOtraEntidadDer + "', " + iIdOrganismoDer.ToString + ", '" + sNomOrganismo + "', '" + sDirOrganismoDer + "', '" + sTelOrganismoDer + "', '" + sFaxOrganismoDer + "', '" + sEmailOrganismoDer + "', '" + sNomProfesional + "', '" + sCargoProfesional + "', '" + sTelProfesional + "', '" + sFaxProfesional + "', '" + sEmailProfesional + "', " + CInt(bComunicacion).ToString + "," + iIdOrgCom.ToString + ", '" + sOtrosOrgCom + "', " + iIdTipologiaASI.ToString + ", " + CInt(bConvivencia).ToString + ", " + CInt(bDenunciado).ToString + ", " + iIdLugarDenuncia.ToString + ", '" + sAtestado + "', '" + sDependenciaPolicial + "', " + iIdMotivoDevolucion.ToString + ", '" + sOtroMotivoDevolucion + "', "

        If Me.dtpFSalidaDev.Checked Then
            sSQL = sSQL + "'" + Format(dtmFSalidaDev, "dd/MM/yyyy") + "', "
        Else
            sSQL = sSQL + " null, "
        End If

        If Me.dtpFEntradaSub.Checked Then
            sSQL = sSQL + "'" + Format(dtmFEntradaSub, "dd/MM/yyyy") + "', "
        Else
            sSQL = sSQL + " null, "
        End If

        sSQL = sSQL + CInt(bAtencionUrgente).ToString + ", "

        If Me.dtpFAtencionUrg.Checked Then
            sSQL = sSQL + "'" + Format(dtmFSolAtencion, "dd/MM/yyyy") + "', "
        Else
            sSQL = sSQL + " null, "
        End If

        If Me.dtpFEnvioFiscalia.Checked Then
            sSQL = sSQL + "'" + Format(dtmFSalidaFiscalia, "dd/MM/yyyy") + "', "
        Else
            sSQL = sSQL + " null, "
        End If

        sSQL = sSQL + CInt(bEnvioConsejeria).ToString + ", "

        If Me.dtpFDerConsejeria.Checked Then
            sSQL = sSQL + "'" + Format(dtmFDerConsejeria, "dd/MM/yyyy") + "', "
        Else
            sSQL = sSQL + " null, "
        End If

        If Me.dtpFechaSalidaDerCIASI.Checked Then
            dtmFSalidaCIASI = Me.dtpFechaSalidaDerCIASI.Value
            sSQL = sSQL + "'" + Format(dtmFSalidaCIASI, "dd/MM/yyyy") + "', '"
        Else
            sSQL = sSQL + " null, '"
        End If

        sSQL = sSQL + UsuarioActual.Nombre + "',  '" + Format(Now, "dd/MM/yyyy HH:mm:ss") + "', '" + UsuarioActual.Nombre + "', '" + Format(Now, "dd/MM/yyyy HH:mm:ss") + "');"

        If Derivacion.InsertarNuevoRegistro(sSQL) Then
            glIdDerivacion = Derivacion.VerIdDerivacion(glIdExpediente)

            If dtmFSalidaDev <> Nothing Then
                frmExpediente.txtFSalDevolucion.Text = Format(dtmFSalidaDev, "dd/MM/yyyy")
            Else
                frmExpediente.txtFSalDevolucion.Text = ""
            End If

            If dtmFEntradaSub <> Nothing Then
                frmExpediente.txtFEntSub.Text = Format(dtmFEntradaSub, "dd/MM/yyyy")
            Else
                frmExpediente.txtFEntSub.Text = ""
            End If

            MessageBox.Show("Datos guardados con éxito", "Guardar datos", MessageBoxButtons.OK, MessageBoxIcon.Information)
            'Si hemos marcado Familia en Riesgo, automáticamente, se generá una alerta para realizar un informe de Valoración de Riesgo Social.  Esta alerta se mostrará en el cuadro alertas sistemáticas y se desactiva al crear el informe sistemático correspondiente
            Crear_Alerta_Valoracion_Riesgo_Social()
        Else
            MessageBox.Show("Ha fallado la inserción del registro", "Guardar datos", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If

    End Sub

    Private Sub Actualizar_Derivacion()
        Dim dtmFEntradaIMFM, dtmFSolAtencion, dtmFAcuseRecibo, dtmFSalidaCIASI, dtmFEntradaCIASI, dtmFSalidaFiscalia, dtmFDerConsejeria, dtmFSalidaDev, dtmFEntradaSub As Date
        Dim bFamiliaRiesgo As Boolean = False
        Dim bComunicacion As Boolean = False
        Dim bConvivencia As Boolean = False
        Dim bDenunciado As Boolean = False
        Dim bAtencionUrgente As Boolean = False
        Dim bEnvioConsejeria As Boolean = False
        Dim iIdEntidadDer, iIdOrganismoDer, iIdTipoInforme, iIdMotivoDevolucion, iIdOrgCom, iIdTipologiaASI, iIdLugarDenuncia As Integer
        Dim sOtraEntidadDer, sNomOrganismo, sDirOrganismoDer, sTelOrganismoDer, sFaxOrganismoDer, sEmailOrganismoDer, sNomProfesional, sCargoProfesional, sEmailProfesional, sTelProfesional, sFaxProfesional, sOtroTipoInforme, sOtroMotivoDevolucion, sSQL, sOtrosOrgCom, sAtestado, sDependenciaPolicial As String

        dtmFEntradaIMFM = Me.dtpFechaEntradaIMFM.Value.Date

        If Me.rbtnRiesgoSI.Checked Then
            bFamiliaRiesgo = True
        Else
            bFamiliaRiesgo = False
        End If

        iIdEntidadDer = CInt(Me.cboEntidadDerivante.SelectedValue)
        sOtraEntidadDer = Me.txtOtraEntidadDerivante.Text.Trim
        iIdOrganismoDer = CInt(Me.cboOrgDerivante.SelectedValue)
        sNomOrganismo = Me.txtNomOrganismo.Text.Trim
        sDirOrganismoDer = Me.txtDirOrgDerivante.Text.Trim
        sTelOrganismoDer = Me.txtTelOrgDerivante.Text.Trim
        sFaxOrganismoDer = Me.txtFaxOrgDerivante.Text.Trim
        sEmailOrganismoDer = Me.txtEmailOrganismo.Text.Trim
        sNomProfesional = Me.txtNomProfDerivante.Text.Trim
        sCargoProfesional = Me.txtCargoProfDerivante.Text.Trim
        sEmailProfesional = Me.txtEmailProfDerivante.Text.Trim
        sTelProfesional = Me.txtTelProfDerivante.Text.Trim
        sFaxProfesional = (Me.txtFaxProfDerivante.Text.Trim)

        If Me.rbtnComSi.Checked Then
            bComunicacion = True
        Else
            bComunicacion = False
        End If

        iIdOrgCom = CInt(Me.cboOrgCom.SelectedValue)
        sOtrosOrgCom = Me.txtOtrosOrgCom.Text.Trim
        iIdTipologiaASI = CInt(Me.cboTipoASI.SelectedValue)

        If Me.rbtnConvSi.Checked Then
            bConvivencia = True
        Else
            bConvivencia = False
        End If

        If Me.rbtnDenunciado.Checked Then
            bDenunciado = True
        Else
            bDenunciado = False
        End If

        iIdLugarDenuncia = CInt(Me.cboLugarDenuncia.SelectedValue)
        sAtestado = Me.txtAtestado.Text.Trim
        sDependenciaPolicial = Me.txtDepPolicial.Text.Trim
        iIdTipoInforme = CInt(Me.cboTipoInforme.SelectedValue)
        sOtroTipoInforme = Me.txtOtroTipoInforme.Text.Trim

        If Me.rbtnUrgenteSi.Checked Then
            bAtencionUrgente = True

            If Me.dtpFAtencionUrg.Checked Then
                dtmFSolAtencion = Me.dtpFAtencionUrg.Value.Date
            Else
                dtmFSolAtencion = Nothing
            End If

        Else
            bAtencionUrgente = False
        End If

        If Me.dtpFSalidaDev.Checked Then
            dtmFSalidaDev = Me.dtpFSalidaDev.Value.Date
        Else
            dtmFSalidaDev = Nothing
        End If

        If Me.dtpFEntradaSub.Checked Then
            dtmFEntradaSub = Me.dtpFEntradaSub.Value.Date
        Else
            dtmFEntradaSub = Nothing
        End If

        If Me.dtpFEnvioFiscalia.Checked Then
            dtmFSalidaFiscalia = Me.dtpFEnvioFiscalia.Value.Date
        Else
            dtmFSalidaFiscalia = Nothing
        End If

        If Me.rbtnConsejeriaSi.Checked Then
            bEnvioConsejeria = True

            If Me.dtpFDerConsejeria.Checked Then
                dtmFDerConsejeria = Me.dtpFDerConsejeria.Value.Date
            Else
                dtmFDerConsejeria = Nothing
            End If

        Else
            bEnvioConsejeria = False
        End If

        sSQL = "UPDATE Derivaciones SET FEntradaIMFM = '" + Format(dtmFEntradaIMFM, "dd/MM/yyyy") + "',  RiesgoFamilia = " + CInt(bFamiliaRiesgo).ToString + ", IdEntidadDerivante =" + iIdEntidadDer.ToString + ", OtraEntidadDerivante = '" + sOtraEntidadDer + "', IdOrganismoDerivante = " + iIdOrganismoDer.ToString + ", NomOrganismo = '" + sNomOrganismo + "', DirOrganismo = '" + sDirOrganismoDer + "', TelOrganismo = '" + sTelOrganismoDer + "', FaxOrganismo = '" + sFaxOrganismoDer + "', EmailOrganismo = '" + sEmailOrganismoDer + "' , ProDerivante = '" + sNomProfesional + "', CargoProDerivante = '" + sCargoProfesional + "', TelProDerivante = '" + sTelProfesional + "', FaxProDerivante = '" + sFaxProfesional + "', EmailProDerivante = '" + sEmailProfesional + "', ComSituacion = " + CInt(bComunicacion).ToString + ", IdOrgCom = " + iIdOrgCom.ToString + ", OtrosOrgCom = '" + sOtrosOrgCom + "', " + "IdTipologiaAsi = " + iIdTipologiaASI.ToString + ", Convivencia = " + CInt(bConvivencia).ToString + ", Denunciado = " + CInt(bDenunciado).ToString + ", IdLugarDenuncia = " + iIdLugarDenuncia.ToString + ", Atestado = '" + sAtestado + "', DependenciaPolicial = '" + sDependenciaPolicial + "', "

        If Me.dtpFechaAcuseRecibo.Checked Then
            dtmFAcuseRecibo = Me.dtpFechaAcuseRecibo.Value
            sSQL = sSQL + "FAcuseRecibo = '" + Format(dtmFAcuseRecibo, "dd/MM/yyyy") + "', "
        Else
            sSQL = sSQL + "FAcuseRecibo = null, "
        End If

        'De momento el nombre del fichero lo vamos a dejar vacio
        sSQL = sSQL + "FicheroAcuseRecibo = '', "

        iIdMotivoDevolucion = CInt(Me.cboMotivoDevProtocolo.SelectedValue)
        sOtroMotivoDevolucion = Me.txtOtroMotivoDevProtocolo.Text.Trim
        dtmFSalidaCIASI = Me.dtpFechaSalidaDerCIASI.Value.Date

        sSQL = sSQL + "IdMotivoDevolucion = " + iIdMotivoDevolucion.ToString + ", OtroMotivoDevolucion = '" + sOtroMotivoDevolucion + "', FSalidaDevolucion = "

        If Me.dtpFSalidaDev.Checked Then
            sSQL = sSQL + "'" + Format(dtmFSalidaDev, "dd/MM/yyyy") + "', "
        Else
            sSQL = sSQL + " null, "
        End If

        sSQL = sSQL + "FEntradaSubsanacion = "

        If Me.dtpFEntradaSub.Checked Then
            sSQL = sSQL + "'" + Format(dtmFEntradaSub, "dd/MM/yyyy") + "', "
        Else
            sSQL = sSQL + " null, "
        End If

        sSQL = sSQL + "AtencionUrgente = " + CInt(bAtencionUrgente).ToString + ", FAtencionUrgente = "

        If Me.dtpFAtencionUrg.Checked Then
            sSQL = sSQL + "'" + Format(dtmFSolAtencion, "dd/MM/yyyy") + "', "
        Else
            sSQL = sSQL + " null, "
        End If

        If Me.dtpFechaSalidaDerCIASI.Checked Then
            sSQL = sSQL + "FSalidaDerCIASI = '" + Format(dtmFSalidaCIASI, "dd/MM/yyyy") + "', "
        Else
            sSQL = sSQL + "FSalidaDerCIASI = null, "
        End If

        If Me.dtpFEnvioFiscalia.Checked Then
            sSQL = sSQL + "FEnvioFiscalia = '" + Format(dtmFSalidaFiscalia, "dd/MM/yyyy") + "', "
        Else
            sSQL = sSQL + "FEnvioFiscalia = null, "
        End If

        sSQL = sSQL + "EnvioConsejeria = " + CInt(bEnvioConsejeria).ToString + ", FEnvioConsejeria = "

        If Me.dtpFDerConsejeria.Checked Then
            sSQL = sSQL + "'" + Format(dtmFDerConsejeria, "dd/MM/yyyy") + "', "
        Else
            sSQL = sSQL + " null, "
        End If

        If Me.dtpFecEntradaDerCIASI.Checked Then
            dtmFEntradaCIASI = Me.dtpFecEntradaDerCIASI.Value.Date
            sSQL = sSQL + "FEntradaDerCIASI = '" + Format(dtmFEntradaCIASI, "dd/MM/yyyy") + "', "
        Else
            sSQL = sSQL + "FEntradaDerCIASI = null, "
        End If

        sSQL = sSQL + "ModificadoPor = '" + UsuarioActual.Nombre + "', FModificadoPor = '" + Format(Now, "dd/MM/yyyy HH:mm:ss") + "' WHERE IdDerivacion = " + glIdDerivacion.ToString + ";"

        If Derivacion.ActualizarDerivacion(sSQL) Then
            MessageBox.Show("Datos de la derivación guardados con éxito", "Actualización de datos", MessageBoxButtons.OK, MessageBoxIcon.Information)

            Me.rbtnRiesgoSI.Enabled = False
            Me.rbtnRiesgoNo.Enabled = False

            If dtmFSalidaDev <> Nothing Then
                frmExpediente.txtFSalDevolucion.Text = Format(dtmFSalidaDev, "dd/MM/yyyy")
            Else
                frmExpediente.txtFSalDevolucion.Text = ""
            End If

            If dtmFEntradaSub <> Nothing Then
                frmExpediente.txtFEntSub.Text = Format(dtmFEntradaSub, "dd/MM/yyyy")
            Else
                frmExpediente.txtFEntSub.Text = ""
            End If

            Crear_Alerta_Valoracion_ASI()
            Crear_Alerta_Valoracion_Riesgo_Social()
        Else
            MessageBox.Show("Ha fallado la actualización de los datos de la derivación", "Actualización de datos", MessageBoxButtons.OK, MessageBoxIcon.Error)
        End If

    End Sub

    Private Sub Cargar_Derivacion(ByVal lIdDerivacion As Long, ByVal lIdExpediente As Long)
        Dim dtDatosDerivacion As DataTable

        dtDatosDerivacion = Derivacion.CargarDerivacion(lIdDerivacion, lIdExpediente)

        Cargar_Datos_Derivacion_En_Controles(dtDatosDerivacion)
        Cargar_Informes_Derivacion()

        bDatosDerivacionCambiados = True
    End Sub

    Private Sub Cargar_Datos_Derivacion_En_Controles(ByVal dtDatosDerivacion As DataTable)
        Dim Fila As DataRow

        Fila = dtDatosDerivacion.Rows(0)

        Me.dtpFechaEntradaIMFM.Value = CDate(Fila.Item("FEntradaIMFM"))

        If CBool(Fila.Item("RiesgoFamilia")) Then
            Me.rbtnRiesgoSI.Checked = True
            Me.rbtnRiesgoSI.Enabled = False
            Me.rbtnRiesgoNo.Enabled = False
            bActivadaValRiesgoSocial = True
        Else
            Me.rbtnRiesgoNo.Checked = True
            bActivadaValRiesgoSocial = False
        End If

        Me.cboEntidadDerivante.SelectedValue = Fila.Item("IdEntidadDerivante")
        Me.txtOtraEntidadDerivante.Text = Fila.Item("OtraEntidadDerivante").ToString
        Me.cboOrgDerivante.SelectedValue = Fila.Item("IdOrganismoDerivante")
        Me.txtNomOrganismo.Text = Fila.Item("NomOrganismo").ToString
        Me.txtDirOrgDerivante.Text = Fila.Item("DirOrganismo").ToString
        Me.txtTelOrgDerivante.Text = Fila.Item("TelOrganismo").ToString
        Me.txtFaxOrgDerivante.Text = Fila.Item("FaxOrganismo").ToString
        Me.txtEmailOrganismo.Text = Fila.Item("EmailOrganismo").ToString
        Me.txtNomProfDerivante.Text = Fila.Item("ProDerivante").ToString
        Me.txtCargoProfDerivante.Text = Fila.Item("CargoProDerivante").ToString
        Me.txtEmailProfDerivante.Text = Fila.Item("EmailProDerivante").ToString
        Me.txtTelProfDerivante.Text = Fila.Item("TelProDerivante").ToString
        Me.txtFaxProfDerivante.Text = Fila.Item("FaxProDerivante").ToString

        If CBool(Fila.Item("ComSituacion")) Then
            Me.rbtnComSi.Checked = True
        Else
            Me.rbtnComNo.Checked = True
        End If

        Me.cboOrgCom.SelectedValue = Fila.Item("IdOrgCom")
        Me.txtOtrosOrgCom.Text = Fila.Item("OtrosOrgCom").ToString

        If Not Fila.Item("EmailProDerivante").ToString.Trim = "" Then
            gsEmailProfesionalDerivante = Fila.Item("Emailorganismo").ToString.Trim
        Else
            gsEmailProfesionalDerivante = ""
        End If

        Me.cboTipoASI.SelectedValue = Fila.Item("IdTipologiaAsi")

        If CBool(Fila.Item("Convivencia")) Then
            Me.rbtnConvSi.Checked = True
        Else
            Me.rbtnConvNo.Checked = True
        End If

        If CBool(Fila.Item("Denunciado")) Then
            Me.rbtnDenunciado.Checked = True
            Me.cboLugarDenuncia.SelectedValue = Fila.Item("IdLugarDenuncia")
            Me.txtAtestado.Text = Fila.Item("Atestado").ToString
            Me.txtDepPolicial.Text = Fila.Item("DependenciaPolicial").ToString
        Else
            Me.rbtnSinDenunciar.Checked = True
        End If

        'Cargar grid tipos de informes si lo hubiera

        If Not Fila.Item("FAcuseRecibo") Is System.DBNull.Value Then
            Me.dtpFechaAcuseRecibo.Value = CDate(Fila.Item("FAcuseRecibo"))
            Me.dtpFechaAcuseRecibo.Format = DateTimePickerFormat.Short
            Me.dtpFechaAcuseRecibo.CustomFormat = ""
            Me.dtpFechaAcuseRecibo.Checked = True
        End If

        If CBool(Fila.Item("AtencionUrgente")) Then
            Me.rbtnUrgenteSi.Checked = True

            If Not Fila.Item("FAtencionUrgente") Is System.DBNull.Value Then
                Me.dtpFAtencionUrg.Value = CDate(Fila.Item("FAtencionUrgente"))
                Me.dtpFAtencionUrg.Format = DateTimePickerFormat.Short
                Me.dtpFAtencionUrg.CustomFormat = ""
                Me.dtpFAtencionUrg.Checked = True
            End If

        Else
            Me.rbtnUrgenteNo.Checked = True
        End If

        Me.cboMotivoDevProtocolo.SelectedValue = CInt(Fila.Item("IdMotivoDevolucion"))
        Me.txtOtroMotivoDevProtocolo.Text = Fila.Item("OtroMotivoDevolucion").ToString

        If Not Fila.Item("FSalidaDevolucion") Is System.DBNull.Value Then
            Me.dtpFSalidaDev.Value = CDate(Fila.Item("FSalidaDevolucion"))
            Me.dtpFSalidaDev.Format = DateTimePickerFormat.Short
            Me.dtpFSalidaDev.CustomFormat = ""
            Me.dtpFSalidaDev.Checked = True
        End If

        If Not Fila.Item("FEntradaSubsanacion") Is System.DBNull.Value Then
            Me.dtpFEntradaSub.Value = CDate(Fila.Item("FEntradaSubsanacion"))
            Me.dtpFEntradaSub.Format = DateTimePickerFormat.Short
            Me.dtpFEntradaSub.CustomFormat = ""
            Me.dtpFEntradaSub.Checked = True
        End If

        If Not Fila.Item("FSalidaDerCIASI") Is System.DBNull.Value Then
            Me.dtpFechaSalidaDerCIASI.Value = CDate(Fila.Item("FSalidaDerCIASI"))
            Me.dtpFechaSalidaDerCIASI.Format = DateTimePickerFormat.Short
            Me.dtpFechaSalidaDerCIASI.CustomFormat = ""
            Me.dtpFechaSalidaDerCIASI.Checked = True
        End If

        If Not Fila.Item("FEnvioFiscalia") Is System.DBNull.Value Then
            Me.dtpFEnvioFiscalia.Value = CDate(Fila.Item("FEnvioFiscalia"))
            Me.dtpFEnvioFiscalia.Format = DateTimePickerFormat.Short
            Me.dtpFEnvioFiscalia.CustomFormat = ""
            Me.dtpFEnvioFiscalia.Checked = True
        End If

        If CBool(Fila.Item("EnvioConsejeria")) Then
            Me.rbtnConsejeriaSi.Checked = True

            If Not Fila.Item("FEnvioConsejeria") Is System.DBNull.Value Then
                Me.dtpFDerConsejeria.Value = CDate(Fila.Item("FEnvioConsejeria"))
                Me.dtpFDerConsejeria.Format = DateTimePickerFormat.Short
                Me.dtpFDerConsejeria.CustomFormat = ""
                Me.dtpFDerConsejeria.Checked = True
            End If

        Else
            Me.rbtnConsejeriaNo.Checked = True
        End If

        If Not Fila.Item("FEntradaDerCIASI") Is System.DBNull.Value Then
            Me.dtpFecEntradaDerCIASI.Value = CDate(Fila.Item("FEntradaDerCIASI"))
            Me.dtpFecEntradaDerCIASI.Format = DateTimePickerFormat.Short
            Me.dtpFecEntradaDerCIASI.CustomFormat = ""
            Me.dtpFecEntradaDerCIASI.Checked = True
            Me.dtpFecEntradaDerCIASI.Enabled = False
        End If

    End Sub

    Private Sub Crear_Alerta_Valoracion_ASI()
        'La alarma se crea automaticamente cuando hemos puesto la fecha de entrada de la derivación en el CIASI.  Hay que tener en cuenta si se ha marcado o no Tramitación Urgente
        Dim bUrgente As Boolean = False

        If Me.dtpFecEntradaDerCIASI.Checked And Me.dtpFecEntradaDerCIASI.Enabled Then

            If IsDate(Me.dtpFecEntradaDerCIASI.Value) Then

                'Comprobamos si la tramitación es urgente o no
                If frmExpediente.rbtnUrgente.Checked Then
                    bUrgente = True
                Else
                    bUrgente = False
                End If

                Activar_Alerta_Valoracion_ASI(bUrgente)
            End If

        End If

    End Sub

    Private Sub Activar_Alerta_Valoracion_ASI(ByVal bUrgente As Boolean)
        Dim dtmFAviso, dtmFLimite As Date
        Dim sSQL, sUrgente As String

        If bUrgente Then
            sUrgente = "Si"
        Else
            sUrgente = "No"
        End If

        dtmFLimite = InfSistematicos.CalcularPlazoLimiteInforme(Me.dtpFecEntradaDerCIASI.Value.Date, TipoInfSistematico.Psicologico, FinalidadPsicologico.ValoracionDelASI)
        dtmFAviso = InfSistematicos.CalcularFechaAlerta(dtmFLimite, TipoInfSistematico.Psicologico, FinalidadPsicologico.ValoracionDelASI)

        sSQL = "INSERT INTO Alarmas_Sis_Expedientes (IdExpediente, IdTipoInforme, IdFinalidad, FAltaAlarma, Urgente, Tipo, Asunto, FAviso, FLimite, Activada, CreadaPor, FechaCreacion) VALUES (" + glIdExpediente.ToString + ", " + CInt(TipoInfSistematico.Psicologico).ToString + ", " + CInt(FinalidadPsicologico.ValoracionDelASI).ToString + ",  '" + Format(Now, "dd/MM/yyyy HH:mm:ss") + "', '" + sUrgente + "', 'Auto', 'Informe psicológico de valoración del ASI', '" + Format(dtmFAviso, "dd/MM/yyyy") + "', '" + Format(dtmFLimite, "dd/MM/yyyy") + "', 1, '" + UsuarioActual.Nombre + "', '" + Format(Now, "dd/MM/yyyy HH:mm:ss") + "');"

        If Alertas.NuevaAlerta(sSQL) Then
            MessageBox.Show("Se ha establecido una alerta para realizar un informe psicológico de valoración del ASI")
            'Actualizar_IdAlerta(glIdExpediente, glIdDerivacion)
            'Esta fecha permanecerá bloqueada
            Me.dtpFecEntradaDerCIASI.Enabled = False
        Else
            MessageBox.Show("Error al crear la alerta")
        End If

    End Sub

    Private Sub Crear_Alerta_Valoracion_Riesgo_Social()
        'Esta alerta se crea cuando hemos marcado que la familia está en situación de riesgo
        If Not bActivadaValRiesgoSocial Then

            If Me.rbtnRiesgoSI.Checked Then
                Activar_Alerta_Valoracion_Riesgo_Social()
            End If

        End If

    End Sub

    Private Sub Activar_Alerta_Valoracion_Riesgo_Social()
        Dim dtmFAviso, dtmFLimite As Date
        Dim sSQL As String

        'En este caso la fecha a partir de la que calculamos la fecha limite es la del dia en que han puesto Familia en Riesgo
        dtmFLimite = InfSistematicos.CalcularPlazoLimiteInforme(Today, TipoInfSistematico.Social, FinalidadSocial.ValoracionRiesgo)
        dtmFAviso = InfSistematicos.CalcularFechaAlerta(dtmFLimite, TipoInfSistematico.Social, FinalidadSocial.ValoracionRiesgo)

        sSQL = "INSERT INTO Alarmas_Sis_Expedientes (IdExpediente, IdTipoInforme, IdFinalidad, FAltaAlarma, Urgente, Tipo, Asunto, FAviso, FLimite, Activada, CreadaPor, FechaCreacion) VALUES (" + glIdExpediente.ToString + ", " + CInt(TipoInfSistematico.Social).ToString + ", " + CInt(FinalidadSocial.ValoracionRiesgo).ToString + ",  '" + Format(Now, "dd/MM/yyyy HH:mm:ss") + "', 'Si', 'auto', 'Informe social de Valoración de Riesgo Social', '" + Format(dtmFAviso, "dd/MM/yyyy") + "', '" + Format(dtmFLimite, "dd/MM/yyyy") + "', 1, '" + UsuarioActual.Nombre + "', '" + Format(Now, "dd/MM/yyyy HH:mm:ss") + "');"

        If Alertas.NuevaAlerta(sSQL) Then
            MessageBox.Show("Se ha establecido una alerta para realizar un informe social de Valoración de Riesgo Social")
            'Actualizar_IdAlerta(glIdExpediente, glIdDerivacion)
            'Esta fecha permanecerá bloqueada
            Me.dtpFecEntradaDerCIASI.Enabled = False
        Else
            MessageBox.Show("Error al crear la alerta")
        End If
    End Sub

    Private Sub Actualizar_IdAlerta(ByVal lIdExpediente As Long, ByVal lIdDerivacion As Long)
        Dim lIdAlerta As Long
        Dim sSQL As String

        lIdAlerta = Alertas.VerIdAlerta(lIdExpediente, 2)

        sSQL = "UPDATE Derivaciones SET IdAlarma = " + lIdAlerta.ToString + " WHERE IdDerivacion = " + lIdDerivacion.ToString + " AND IdExpediente = " + lIdExpediente.ToString + ";"

        Derivacion.ActualizarDerivacion(sSQL)
    End Sub

    Private Sub Autocompletar_Datos_Ultima_Derivacion()
        Me.cboEntidadDerivante.SelectedValue = DatosUltimaDerivacion.Entidad
        Me.txtOtraEntidadDerivante.Text = DatosUltimaDerivacion.OtraEntidad
        Me.cboOrgDerivante.SelectedValue = DatosUltimaDerivacion.Organismo
        Me.txtNomOrganismo.Text = DatosUltimaDerivacion.NomOrganismo
        Me.txtDirOrgDerivante.Text = DatosUltimaDerivacion.DirOrganismo
        Me.txtTelOrgDerivante.Text = DatosUltimaDerivacion.TelOrganismo
        Me.txtFaxOrgDerivante.Text = DatosUltimaDerivacion.FaxOrganismo
        Me.txtEmailOrganismo.Text = DatosUltimaDerivacion.EmailOrganismo
        Me.txtNomProfDerivante.Text = DatosUltimaDerivacion.NomProfesional
        Me.txtCargoProfDerivante.Text = DatosUltimaDerivacion.CargoProfesional
        Me.txtTelProfDerivante.Text = DatosUltimaDerivacion.TelProfesional
        Me.txtFaxOrgDerivante.Text = DatosUltimaDerivacion.FaxProfesional
        Me.txtEmailProfDerivante.Text = DatosUltimaDerivacion.EmailProfesional

        If DatosUltimaDerivacion.ComOtroOrganismo Then
            Me.rbtnComSi.Checked = True
        Else
            Me.rbtnComNo.Checked = True
        End If

        Me.cboOrgCom.SelectedValue = DatosUltimaDerivacion.ComOrganismo
        Me.txtOtrosOrgCom.Text = DatosUltimaDerivacion.ComNomOtroOrganismo
    End Sub

    Private Sub Aplicar_Perfiles()

        Select Case UsuarioActual.Grupo
            Case "system"    'Grupo con todos los privilegios.  Desbloquear todos los controles
                Aplicar_Perfil_SYSTEM()
            Case "imfm"
                Aplicar_Perfil_IMFM()
            Case "ciasi"
                Aplicar_Perfil_CIASI()
        End Select

    End Sub

    Private Sub Aplicar_Perfil_IMFM()

        Select Case UsuarioActual.Rol
            Case "administrativo"
                Aplicar_Rol_Administrativo_IMFM()
            Case "tecnico"
                Aplicar_Rol_Tecnico_IMFM()
        End Select

    End Sub

    Private Sub Aplicar_Rol_Administrativo_IMFM()
        Me.rbtnRiesgoSI.Enabled = True
        Me.rbtnRiesgoNo.Enabled = True
        Me.dtpFechaEntradaIMFM.Enabled = True
        Me.cboEntidadDerivante.Enabled = True
        Me.txtOtraEntidadDerivante.Enabled = True
        Me.cboOrgDerivante.Enabled = True
        Me.txtNomOrganismo.Enabled = True
        Me.txtDirOrgDerivante.Enabled = True
        Me.txtTelOrgDerivante.Enabled = True
        Me.txtFaxOrgDerivante.Enabled = True
        Me.txtEmailOrganismo.Enabled = True
        Me.txtNomProfDerivante.Enabled = True
        Me.txtCargoProfDerivante.Enabled = True
        Me.txtEmailProfDerivante.Enabled = True
        Me.txtTelProfDerivante.Enabled = True
        Me.txtFaxProfDerivante.Enabled = True
        Me.rbtnComSi.Enabled = True
        Me.rbtnComNo.Enabled = True
        Me.cboOrgCom.Enabled = True
        Me.txtOtrosOrgCom.Enabled = True
        Me.txtTelOrgDerivante.Enabled = True
        Me.txtFaxOrgDerivante.Enabled = True
        Me.cboTipoASI.Enabled = True
        Me.rbtnConvSi.Enabled = True
        Me.rbtnConvNo.Enabled = True
        Me.rbtnDenunciado.Enabled = True
        Me.rbtnSinDenunciar.Enabled = True
        Me.cboLugarDenuncia.Enabled = True
        Me.txtAtestado.Enabled = True
        Me.txtDepPolicial.Enabled = True
        Me.cboTipoInforme.Enabled = True
        Me.txtOtroTipoInforme.Enabled = True
        Me.btnSelArchivo.Enabled = True
        Me.btnSubirArchivo.Enabled = True
        Me.rbtnUrgenteSi.Enabled = True
        Me.rbtnUrgenteNo.Enabled = True
        Me.dtpFAtencionUrg.Enabled = True
        Me.dtpFechaAcuseRecibo.Enabled = True
        Me.btnEnviarAcuseRecibo.Enabled = True
        Me.cboMotivoDevProtocolo.Enabled = True
        Me.txtOtroMotivoDevProtocolo.Enabled = True
        Me.dtpFSalidaDev.Enabled = True
        Me.dtpFEntradaSub.Enabled = True
        Me.dtpFechaSalidaDerCIASI.Enabled = True
        Me.dtpFEnvioFiscalia.Enabled = True
        Me.rbtnConsejeriaSi.Enabled = True
        Me.rbtnConsejeriaNo.Enabled = True
        Me.dtpFDerConsejeria.Enabled = True
        Me.dtpFecEntradaDerCIASI.Enabled = False
        Me.picGuardar.Enabled = True
        Me.txtArchivoSeleccionado.Enabled = False
        Me.txtArchivoSeleccionado.BackColor = Color.White
    End Sub

    Private Sub Aplicar_Rol_Tecnico_IMFM()
        Me.rbtnRiesgoSI.Enabled = False
        Me.rbtnRiesgoNo.Enabled = False
        Me.dtpFechaEntradaIMFM.Enabled = False
        Me.cboEntidadDerivante.Enabled = False
        Me.cboEntidadDerivante.BackColor = Color.White
        Me.txtOtraEntidadDerivante.Enabled = False
        Me.txtOtraEntidadDerivante.BackColor = Color.White
        Me.cboOrgDerivante.Enabled = False
        Me.cboOrgDerivante.DropDownStyle = ComboBoxStyle.DropDownList
        Me.cboOrgDerivante.BackColor = Color.White
        Me.txtNomOrganismo.Enabled = False
        Me.txtNomOrganismo.BackColor = Color.White
        Me.txtDirOrgDerivante.Enabled = False
        Me.txtDirOrgDerivante.BackColor = Color.White
        Me.txtTelOrgDerivante.Enabled = False
        Me.txtTelOrgDerivante.BackColor = Color.White
        Me.txtFaxOrgDerivante.Enabled = False
        Me.txtFaxOrgDerivante.BackColor = Color.White
        Me.txtEmailOrganismo.Enabled = False
        Me.txtEmailOrganismo.BackColor = Color.White
        Me.txtNomProfDerivante.Enabled = False
        Me.txtNomProfDerivante.BackColor = Color.White
        Me.txtCargoProfDerivante.Enabled = False
        Me.txtCargoProfDerivante.BackColor = Color.White
        Me.txtTelProfDerivante.Enabled = False
        Me.txtTelProfDerivante.BackColor = Color.White
        Me.txtFaxProfDerivante.Enabled = False
        Me.txtFaxProfDerivante.BackColor = Color.White
        Me.txtEmailProfDerivante.Enabled = False
        Me.txtEmailProfDerivante.BackColor = Color.White
        Me.rbtnComSi.Enabled = False
        Me.rbtnComNo.Enabled = False
        Me.cboOrgCom.Enabled = False
        Me.cboOrgCom.BackColor = Color.White
        Me.txtOtrosOrgCom.Enabled = False
        Me.txtOtrosOrgCom.BackColor = Color.White
        Me.txtTelOrgDerivante.Enabled = False
        Me.txtTelOrgDerivante.BackColor = Color.White
        Me.txtFaxOrgDerivante.Enabled = False
        Me.txtFaxOrgDerivante.BackColor = Color.White
        Me.cboTipoASI.Enabled = False
        Me.cboTipoASI.BackColor = Color.White
        Me.rbtnConvSi.Enabled = False
        Me.rbtnConvNo.Enabled = False
        Me.rbtnDenunciado.Enabled = False
        Me.rbtnSinDenunciar.Enabled = False
        Me.cboLugarDenuncia.Enabled = False
        Me.cboLugarDenuncia.BackColor = Color.White
        Me.txtAtestado.Enabled = False
        Me.txtAtestado.BackColor = Color.White
        Me.txtDepPolicial.Enabled = False
        Me.txtDepPolicial.BackColor = Color.White
        Me.cboTipoInforme.Enabled = False
        Me.cboTipoInforme.DropDownStyle = ComboBoxStyle.DropDownList
        Me.cboTipoInforme.BackColor = Color.White
        Me.txtOtroTipoInforme.Enabled = False
        Me.txtOtroTipoInforme.BackColor = Color.White
        Me.btnSelArchivo.Enabled = False
        Me.btnSubirArchivo.Enabled = False
        Me.rbtnUrgenteSi.Enabled = False
        Me.rbtnUrgenteNo.Enabled = False
        Me.dtpFAtencionUrg.Enabled = False
        Me.dtpFechaAcuseRecibo.Enabled = False
        Me.btnEnviarAcuseRecibo.Enabled = False
        Me.cboMotivoDevProtocolo.Enabled = False
        Me.cboMotivoDevProtocolo.BackColor = Color.White
        Me.txtOtroMotivoDevProtocolo.Enabled = False
        Me.txtOtroMotivoDevProtocolo.BackColor = Color.White
        Me.dtpFSalidaDev.Enabled = False
        Me.dtpFEntradaSub.Enabled = False
        Me.dtpFechaSalidaDerCIASI.Enabled = False
        Me.dtpFEnvioFiscalia.Enabled = False
        Me.rbtnConsejeriaSi.Enabled = False
        Me.rbtnConsejeriaNo.Enabled = False
        Me.dtpFDerConsejeria.Enabled = False
        Me.dtpFecEntradaDerCIASI.Enabled = False
        Me.picGuardar.Enabled = False
        Me.txtArchivoSeleccionado.Enabled = False
        Me.txtArchivoSeleccionado.BackColor = Color.White
    End Sub

    Private Sub Aplicar_Perfil_CIASI()
        Me.rbtnRiesgoSI.Enabled = False
        Me.rbtnRiesgoNo.Enabled = False
        Me.dtpFechaEntradaIMFM.Enabled = False
        Me.cboEntidadDerivante.Enabled = False
        Me.cboEntidadDerivante.BackColor = Color.White
        Me.txtOtraEntidadDerivante.Enabled = False
        Me.txtOtraEntidadDerivante.BackColor = Color.White
        Me.cboOrgDerivante.DropDownStyle = ComboBoxStyle.DropDownList
        Me.cboOrgDerivante.Enabled = False
        Me.cboOrgDerivante.BackColor = Color.White
        Me.txtNomOrganismo.Enabled = False
        Me.txtNomOrganismo.BackColor = Color.White
        Me.txtDirOrgDerivante.Enabled = False
        Me.txtDirOrgDerivante.BackColor = Color.White
        Me.txtTelOrgDerivante.Enabled = False
        Me.txtTelOrgDerivante.BackColor = Color.White
        Me.txtFaxOrgDerivante.Enabled = False
        Me.txtFaxOrgDerivante.BackColor = Color.White
        Me.txtEmailOrganismo.Enabled = False
        Me.txtEmailOrganismo.BackColor = Color.White
        Me.txtNomProfDerivante.Enabled = False
        Me.txtNomProfDerivante.BackColor = Color.White
        Me.txtCargoProfDerivante.Enabled = False
        Me.txtCargoProfDerivante.BackColor = Color.White
        Me.txtEmailProfDerivante.Enabled = False
        Me.txtEmailProfDerivante.BackColor = Color.White
        Me.txtTelProfDerivante.Enabled = False
        Me.txtTelProfDerivante.BackColor = Color.White
        Me.txtFaxProfDerivante.Enabled = False
        Me.txtFaxProfDerivante.BackColor = Color.White
        Me.rbtnComSi.Enabled = False
        Me.rbtnComNo.Enabled = False
        Me.cboOrgCom.Enabled = False
        Me.cboOrgCom.BackColor = Color.White
        Me.txtOtrosOrgCom.Enabled = False
        Me.txtOtrosOrgCom.BackColor = Color.White
        Me.txtTelOrgDerivante.Enabled = False
        Me.txtTelOrgDerivante.BackColor = Color.White
        Me.txtFaxOrgDerivante.Enabled = False
        Me.txtFaxOrgDerivante.BackColor = Color.White
        Me.cboTipoASI.Enabled = False
        Me.cboTipoASI.BackColor = Color.White
        Me.rbtnConvSi.Enabled = False
        Me.rbtnConvNo.Enabled = False
        Me.rbtnDenunciado.Enabled = False
        Me.rbtnSinDenunciar.Enabled = False
        Me.cboLugarDenuncia.Enabled = False
        Me.cboLugarDenuncia.BackColor = Color.White
        Me.txtAtestado.Enabled = False
        Me.txtAtestado.BackColor = Color.White
        Me.txtDepPolicial.Enabled = False
        Me.txtDepPolicial.BackColor = Color.White
        Me.cboTipoInforme.Enabled = False
        Me.cboTipoInforme.BackColor = Color.White
        Me.cboTipoInforme.DropDownStyle = ComboBoxStyle.DropDownList
        Me.txtOtroTipoInforme.Enabled = False
        Me.txtOtroTipoInforme.BackColor = Color.White
        Me.btnSelArchivo.Enabled = False
        Me.btnSubirArchivo.Enabled = False
        Me.rbtnUrgenteSi.Enabled = False
        Me.rbtnUrgenteNo.Enabled = False
        Me.dtpFAtencionUrg.Enabled = False
        Me.dtpFechaAcuseRecibo.Enabled = False
        Me.btnEnviarAcuseRecibo.Enabled = False
        Me.cboMotivoDevProtocolo.Enabled = False
        Me.cboMotivoDevProtocolo.BackColor = Color.White
        Me.txtOtroMotivoDevProtocolo.Enabled = False
        Me.txtOtroMotivoDevProtocolo.BackColor = Color.White
        Me.dtpFSalidaDev.Enabled = False
        Me.dtpFEntradaSub.Enabled = False
        Me.dtpFechaSalidaDerCIASI.Enabled = False
        Me.dtpFEnvioFiscalia.Enabled = False
        Me.rbtnConsejeriaSi.Enabled = False
        Me.rbtnConsejeriaNo.Enabled = False
        Me.dtpFDerConsejeria.Enabled = False
        Me.dtpFecEntradaDerCIASI.Enabled = True
        Me.picGuardar.Enabled = True
        Me.txtArchivoSeleccionado.Enabled = False
        Me.txtArchivoSeleccionado.BackColor = Color.White
    End Sub

    Private Sub Aplicar_Perfil_SYSTEM()
        Me.rbtnRiesgoSI.Enabled = True
        Me.rbtnRiesgoNo.Enabled = True
        Me.dtpFechaEntradaIMFM.Enabled = True
        Me.cboEntidadDerivante.Enabled = True
        Me.txtOtraEntidadDerivante.Enabled = True
        Me.cboOrgDerivante.Enabled = True
        Me.txtNomOrganismo.Enabled = True
        Me.txtDirOrgDerivante.Enabled = True
        Me.txtTelOrgDerivante.Enabled = True
        Me.txtFaxOrgDerivante.Enabled = True
        Me.txtEmailOrganismo.Enabled = True
        Me.txtNomProfDerivante.Enabled = True
        Me.txtCargoProfDerivante.Enabled = True
        Me.txtTelProfDerivante.Enabled = True
        Me.txtFaxProfDerivante.Enabled = True
        Me.txtEmailProfDerivante.Enabled = True
        Me.rbtnComSi.Enabled = True
        Me.rbtnComNo.Enabled = True
        Me.cboOrgCom.Enabled = True
        Me.txtOtrosOrgCom.Enabled = True
        Me.txtTelOrgDerivante.Enabled = True
        Me.txtFaxOrgDerivante.Enabled = True
        Me.cboTipoASI.Enabled = True
        Me.rbtnComSi.Enabled = True
        Me.rbtnComNo.Enabled = True
        Me.rbtnDenunciado.Enabled = True
        Me.rbtnSinDenunciar.Enabled = True
        Me.cboLugarDenuncia.Enabled = True
        Me.txtAtestado.Enabled = True
        Me.txtDepPolicial.Enabled = True
        Me.rbtnUrgenteSi.Enabled = True
        Me.rbtnUrgenteNo.Enabled = True
        Me.dtpFAtencionUrg.Enabled = True
        Me.cboTipoInforme.Enabled = True
        Me.txtOtroTipoInforme.Enabled = True
        Me.btnSelArchivo.Enabled = True
        Me.btnSubirArchivo.Enabled = True
        Me.dtpFechaAcuseRecibo.Enabled = True
        Me.btnEnviarAcuseRecibo.Enabled = True
        Me.cboMotivoDevProtocolo.Enabled = True
        Me.txtOtroMotivoDevProtocolo.Enabled = True
        Me.dtpFSalidaDev.Enabled = True
        Me.dtpFEntradaSub.Enabled = True
        Me.dtpFechaSalidaDerCIASI.Enabled = True
        Me.dtpFEnvioFiscalia.Enabled = True
        Me.rbtnConsejeriaSi.Enabled = True
        Me.rbtnConsejeriaNo.Enabled = True
        Me.dtpFDerConsejeria.Enabled = True
        Me.dtpFecEntradaDerCIASI.Enabled = True
        Me.picGuardar.Enabled = True
        Me.txtArchivoSeleccionado.Enabled = False
        Me.txtArchivoSeleccionado.BackColor = Color.White
    End Sub

    Private Sub Seleccionar_Archivo()

        If glIdDerivacion > 0 Then
            'Ruta donde están los archivos pdf que hemos creado (PC)
            'Me.ofdSeleccionarArchivo.InitialDirectory = "D:\Documentos_Escaneados_Pdf"
            'Ruta donde están los archivos pdf que hemos creado (Portatil)
            'Me.ofdSeleccionarArchivo.InitialDirectory = "C:\Documentos_Imfm_ciasi\Documentos_Escaneados_Pdf"
            Me.ofdSeleccionarArchivo.InitialDirectory = gsDirectorioInicial
            Me.ofdSeleccionarArchivo.FileName = ""
            Me.ofdSeleccionarArchivo.Filter = "Archivos pdf (*.pdf)|*.pdf|All files (*.*)|*.*"
            Me.ofdSeleccionarArchivo.FilterIndex = 1
            Me.ofdSeleccionarArchivo.Title = "Seleccionar informe (archivo pdf)"

            If ofdSeleccionarArchivo.ShowDialog = Windows.Forms.DialogResult.OK Then
                Me.txtArchivoSeleccionado.Text = Me.ofdSeleccionarArchivo.SafeFileName

                sRutaArchivoOrigen = Me.ofdSeleccionarArchivo.FileName
                sNombreArchivo = Me.ofdSeleccionarArchivo.SafeFileName
            End If

        Else
            MessageBox.Show("Debe guardar primero los datos antes de poder adjuntar un archivo", "Adjuntar informe derivación", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If

    End Sub

    Private Sub Subir_Archivo()
        Dim sNombreFinalArchivo As String = ""
        Dim sDirectorioDestino As String = ""

        If sNombreArchivo <> "" Then
            'Aquí tendremos que hacer dos cosas:
            '1º) Copiar el archivo a su carpeta definitiva.  Según el tipo de informe irá a una carpeta u otra

            Select Case CInt(Me.cboTipoInforme.SelectedValue)
                Case 1 'Ninguno
                    MessageBox.Show("Debe seleccionar un tipo de archivo", "Seleccionar archivo", MessageBoxButtons.OK, MessageBoxIcon.Information)

                    Exit Sub
                Case 2 'Medico
                    sDirectorioDestino = gsDirFinalDerMedicos
                Case 3 'Social
                    sDirectorioDestino = gsDirFinalDerSociales
                Case 4 'Denuncia
                    sDirectorioDestino = gsDirFinalDerDenuncias
                Case 5 'Atestado
                    sDirectorioDestino = gsDirFinalDerAtestados
                Case 7 'Derivacion
                    sDirectorioDestino = gsDirFinalDerDerivaciones
                Case 8 'Otros
                    sDirectorioDestino = gsDirFinalDerOtros
            End Select

            'Tenemos que componer un nombre definitivo para el archivo basandonos en la numeracion del expediente y en una cadena campuesta por el dia y la hora actuales

            'sNombreFinalArchivo = Mid(frmExpediente.lblNumExpediente.Text, 1, InStr(frmExpediente.lblNumExpediente.Text, " ") - 1)
            sNombreFinalArchivo = frmExpediente.lblNumExpediente.Text.Trim
            'Eliminamos el caracter / que no puede formar parte del nombre del archivo
            sNombreFinalArchivo = Replace(sNombreFinalArchivo, "/", "-", , 1)
            'Al nombre definitivo le añadimos la fecha y hora actuales y solo serán archivos pdf
            sNombreFinalArchivo = Microsoft.VisualBasic.Left(frmExpediente.lblTipoDocumento.Text, 1) + sNombreFinalArchivo + Format(Now, "dd-MM-yyyy HH-mm-ss") + ".pdf"

            'Copiamos el archivo a su ubicación definitiva
            'Ruta cuando usamos el PC
            My.Computer.FileSystem.CopyFile(sRutaArchivoOrigen, sDirectorioDestino + sNombreFinalArchivo)
            'Ruta cuando usamos el portatil
            'My.Computer.FileSystem.CopyFile(Me.txtArchivoSeleccionado.Text.Trim, "C:\Area de Proyectos\Visual Studio 2005\Imfm_ciasi\Documentos_Escaneados_Pdf\" + sNombreFinalArchivo)

            'Para trabajar con archivos.
            'My.Computer.FileSystem.CopyFile()
            'También se puede hacer lo anterior con el objeto File.Copy

            '2) Añadir el informe a la tabla de informes de la derivación
            If AdjuntarInformeDerivacion(CInt(glIdDerivacion), CInt(Me.cboTipoInforme.SelectedValue), Me.txtOtroTipoInforme.Text.Trim, sNombreFinalArchivo) Then
                Me.txtArchivoSeleccionado.Text = sNombreFinalArchivo

                MessageBox.Show("Informe subido con éxito", "Subir informe derivacion", MessageBoxButtons.OK, MessageBoxIcon.Information)

                Cargar_Informes_Derivacion()
            Else
                MessageBox.Show("El informe no ha podido ser subido." + vbCrLf + "Por favor, reinténtelo dentro de unos segundos", "Subir informe derivación", MessageBoxButtons.OK, MessageBoxIcon.Error)
            End If

        Else
            MessageBox.Show("Antes de poder subir un archivo debe seleccionarlo", "Subir archivo", MessageBoxButtons.OK, MessageBoxIcon.Information)
        End If
    End Sub

    Private Sub Enviar_Acuse_Recibo()
        gsEmailProfesionalDerivante = Me.txtEmailProfDerivante.Text.Trim

        frmEmail.ShowDialog()
    End Sub

    Public Sub Cargar_Datos_Organismo_En_Controles(ByVal dtDatos As DataTable)
        Dim Fila As DataRow

        Fila = dtDatos.Rows(0)

        Me.txtNomOrganismo.Text = ""
        Me.txtDirOrgDerivante.Text = Fila.Item("Direccion").ToString
        Me.txtTelOrgDerivante.Text = Fila.Item("Telefono").ToString
        Me.txtFaxOrgDerivante.Text = Fila.Item("Fax").ToString
        Me.txtEmailOrganismo.Text = Fila.Item("Email").ToString

        'If Not Fila.Item("Email").ToString.Trim = "" Then
        '    gsEmailEntidadDerivante = Fila.Item("Email").ToString.Trim
        'Else
        '    gsEmailEntidadDerivante = ""
        'End If

    End Sub
#End Region


End Class