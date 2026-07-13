import { Component, OnInit, signal, viewChild } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { AdminProjectService } from '../api/admin-project.service';
import { AdminProjectDto, StackOptionDto } from '../types/admin-project.types';
import { StacksAdminComponent } from './stacks-admin.component';
import { API_URL } from '../../../services-conf/api-config';

@Component({
    selector: 'app-projects-admin',
    standalone: true,
    imports: [FormsModule, CommonModule, StacksAdminComponent],
    templateUrl: '../ui/projects-admin.component.html',
    styleUrls: ['../../admin/css/dashboard.component.css']
})
export class ProjectsAdminComponent implements OnInit {
    // Lista
    projects = signal<AdminProjectDto[]>([]);
    loading = signal(true);
    error = signal<string | null>(null);

    // Nuevo proyecto
    showForm = signal(false);
    editingId = signal<number | null>(null);

    formTitle = signal('');
    formDescription = signal('');
    formStartDate = signal('');
    formEndDate = signal('');
    formIsActive = signal(true);
    formRepositoryUrl = signal('');
    formType = signal('Personal');
    formImageFile = signal<File | null>(null);
    formStackIds = signal<number[]>([]);
    formSaving = signal(false);
    formSaveMsg = signal<string | null>(null);

    // Stacks disponibles
    stackOptions = signal<StackOptionDto[]>([]);

    // Imagen existente
    existingImageUrl = signal<string | null>(null);
    removeImage = signal(false);

    projectTypes = ['Personal', 'Professional', 'Training'];

    constructor(
        private service: AdminProjectService,
        private http: HttpClient
    ) { }

    ngOnInit(): void {
        this.loadProjects();
        this.loadStackOptions();
    }

    loadProjects(): void {
        this.loading.set(true);
        this.error.set(null);
        this.service.getAll().subscribe({
            next: (list) => { this.projects.set(list); this.loading.set(false); },
            error: () => { this.error.set('No se pudieron cargar los proyectos.'); this.loading.set(false); }
        });
    }

    loadStackOptions(): void {
        this.http.get<StackOptionDto[]>(`${API_URL}/stack`).subscribe({
            next: (list) => this.stackOptions.set(list),
            error: () => console.error('Error al cargar stacks')
        });
    }

    openCreate(): void {
        this.resetForm();
        this.showForm.set(true);
        this.editingId.set(null);
    }

    openEdit(project: AdminProjectDto): void {
        this.resetForm();
        this.editingId.set(project.id);
        this.showForm.set(true);
        this.formTitle.set(project.title);
        this.formDescription.set(project.description ?? '');
        this.formStartDate.set(project.startDate?.substring(0, 10) ?? '');
        this.formEndDate.set(project.endDate?.substring(0, 10) ?? '');
        this.formIsActive.set(project.isActive);
        this.formRepositoryUrl.set(project.repositoryUrl ?? '');
        this.formType.set(project.type);
        this.existingImageUrl.set(project.imageUrl);
        this.formStackIds.set(project.stacks.map(s => s.id));
    }

    cancelForm(): void {
        this.showForm.set(false);
        this.editingId.set(null);
        this.resetForm();
    }

    toggleStack(stackId: number): void {
        const current = this.formStackIds();
        if (current.includes(stackId)) {
            this.formStackIds.set(current.filter(id => id !== stackId));
        } else {
            this.formStackIds.set([...current, stackId]);
        }
    }

    onImageSelected(event: Event): void {
        const input = event.target as HTMLInputElement;
        if (!input.files?.length) return;
        this.formImageFile.set(input.files[0]);
        this.removeImage.set(false);
        input.value = '';
    }

    removeCurrentImage(): void {
        this.existingImageUrl.set(null);
        this.formImageFile.set(null);
        this.removeImage.set(true);
    }

    save(): void {
        if (!this.formTitle().trim()) return;

        this.formSaving.set(true);
        this.formSaveMsg.set(null);

        const formData = new FormData();
        formData.append('title', this.formTitle());
        formData.append('description', this.formDescription());
        formData.append('startDate', this.formStartDate());
        formData.append('endDate', this.formEndDate());
        formData.append('isActive', String(this.formIsActive()));
        formData.append('repositoryUrl', this.formRepositoryUrl());
        formData.append('type', this.formType());
        formData.append('removeImage', String(this.removeImage()));

        const img = this.formImageFile();
        if (img) {
            formData.append('imageFile', img);
        }

        this.formStackIds().forEach(id => formData.append('stackIds', String(id)));

        const editId = this.editingId();
        if (editId !== null) {
            this.service.update(editId, formData).subscribe({
                next: (updated) => {
                    this.projects.set(this.projects().map(p => p.id === updated.id ? updated : p));
                    this.cancelForm();
                    this.formSaving.set(false);
                },
                error: (err) => {
                    this.formSaveMsg.set(err?.error?.message ?? 'Error al actualizar proyecto.');
                    this.formSaving.set(false);
                    setTimeout(() => this.formSaveMsg.set(null), 3000);
                }
            });
        } else {
            this.service.create(formData).subscribe({
                next: (created) => {
                    this.projects.set([...this.projects(), created]);
                    this.cancelForm();
                    this.formSaving.set(false);
                },
                error: (err) => {
                    this.formSaveMsg.set(err?.error?.message ?? 'Error al crear proyecto.');
                    this.formSaving.set(false);
                    setTimeout(() => this.formSaveMsg.set(null), 3000);
                }
            });
        }
    }

    deleteProject(project: AdminProjectDto): void {
        if (!confirm(`¿Eliminar "${project.title}"?`)) return;
        this.service.delete(project.id).subscribe({
            next: () => this.projects.set(this.projects().filter(p => p.id !== project.id)),
            error: () => {
                this.formSaveMsg.set('Error al eliminar proyecto.');
                setTimeout(() => this.formSaveMsg.set(null), 3000);
            }
        });
    }

    private resetForm(): void {
        this.formTitle.set('');
        this.formDescription.set('');
        this.formStartDate.set('');
        this.formEndDate.set('');
        this.formIsActive.set(true);
        this.formRepositoryUrl.set('');
        this.formType.set('Personal');
        this.formImageFile.set(null);
        this.formStackIds.set([]);
        this.existingImageUrl.set(null);
        this.removeImage.set(false);
        this.formSaveMsg.set(null);
    }
}