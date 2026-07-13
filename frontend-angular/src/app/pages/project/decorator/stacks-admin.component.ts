import { Component, OnInit, signal, Output, EventEmitter } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { CommonModule } from '@angular/common';
import { HttpClient } from '@angular/common/http';
import { AdminStackService } from '../api/admin-stack.service';
import { AdminStackDto } from '../types/admin-stack.types';
import { SkillDto } from '../../admin/types/dashboard.types';
import { API_URL } from '../../../services-conf/api-config';

@Component({
    selector: 'app-stacks-admin',
    standalone: true,
    imports: [FormsModule, CommonModule],
    templateUrl: '../ui/stacks-admin.component.html',
    styleUrls: ['../../admin/css/dashboard.component.css', '../css/stacks-admin.css']
})
export class StacksAdminComponent implements OnInit {
    stacks = signal<AdminStackDto[]>([]);
    loading = signal(true);
    error = signal<string | null>(null);

    // Skills disponibles para asignar
    availableSkills = signal<SkillDto[]>([]);

    // Formulario
    showForm = signal(false);
    editingId = signal<number | null>(null);
    formSummary = signal('');
    formCategory = signal('');
    formIsActive = signal(true);
    formSkillIds = signal<number[]>([]);
    formSaving = signal(false);
    formSaveMsg = signal<string | null>(null);

    categories = ['Frontend', 'Backend', 'Database', 'DevOps', 'Cloud', 'Mobile', 'Other'];

    @Output() stacksChanged = new EventEmitter<void>();

    constructor(
        private service: AdminStackService,
        private http: HttpClient
    ) { }

    ngOnInit(): void {
        this.loadStacks();
        this.loadSkills();
    }

    loadStacks(): void {
        this.loading.set(true);
        this.error.set(null);
        this.service.getAll().subscribe({
            next: (list) => { this.stacks.set(list); this.loading.set(false); },
            error: () => { this.error.set('No se pudieron cargar los stacks.'); this.loading.set(false); }
        });
    }

    loadSkills(): void {
        this.http.get<SkillDto[]>(`${API_URL}/skill`).subscribe({
            next: (list) => this.availableSkills.set(list.filter(s => s.isActive)),
            error: () => console.error('Error al cargar skills')
        });
    }

    openCreate(): void {
        this.resetForm();
        this.showForm.set(true);
        this.editingId.set(null);
    }

    openEdit(stack: AdminStackDto): void {
        this.resetForm();
        this.editingId.set(stack.id);
        this.showForm.set(true);
        this.formSummary.set(stack.summary);
        this.formCategory.set(stack.category);
        this.formIsActive.set(stack.isActive);
        this.formSkillIds.set(stack.skills.map(s => s.id));
    }

    cancelForm(): void {
        this.showForm.set(false);
        this.editingId.set(null);
        this.resetForm();
    }

    toggleSkill(skillId: number): void {
        const current = this.formSkillIds();
        if (current.includes(skillId)) {
            this.formSkillIds.set(current.filter(id => id !== skillId));
        } else {
            this.formSkillIds.set([...current, skillId]);
        }
    }

    save(): void {
        if (!this.formSummary().trim()) return;

        this.formSaving.set(true);
        this.formSaveMsg.set(null);

        const payload = {
            summary: this.formSummary(),
            category: this.formCategory(),
            isActive: this.formIsActive(),
            skillIds: this.formSkillIds()
        };

        const editId = this.editingId();
        if (editId !== null) {
            this.service.update(editId, payload).subscribe({
                next: (updated) => {
                    this.stacks.set(this.stacks().map(s => s.id === updated.id ? updated : s));
                    this.cancelForm();
                    this.formSaving.set(false);
                    this.stacksChanged.emit();
                },
                error: (err) => {
                    this.formSaveMsg.set(err?.error?.message ?? 'Error al actualizar stack.');
                    this.formSaving.set(false);
                    setTimeout(() => this.formSaveMsg.set(null), 3000);
                }
            });
        } else {
            this.service.create(payload).subscribe({
                next: (created) => {
                    this.stacks.set([...this.stacks(), created]);
                    this.cancelForm();
                    this.formSaving.set(false);
                    this.stacksChanged.emit();
                },
                error: (err) => {
                    this.formSaveMsg.set(err?.error?.message ?? 'Error al crear stack.');
                    this.formSaving.set(false);
                    setTimeout(() => this.formSaveMsg.set(null), 3000);
                }
            });
        }
    }

    deleteStack(stack: AdminStackDto): void {
        if (!confirm(`¿Eliminar el stack "${stack.summary}"?`)) return;
        this.service.delete(stack.id).subscribe({
            next: () => {
                this.stacks.set(this.stacks().filter(s => s.id !== stack.id));
                this.stacksChanged.emit();
            },
            error: () => {
                this.formSaveMsg.set('Error al eliminar stack.');
                setTimeout(() => this.formSaveMsg.set(null), 3000);
            }
        });
    }

    private resetForm(): void {
        this.formSummary.set('');
        this.formCategory.set('Frontend');
        this.formIsActive.set(true);
        this.formSkillIds.set([]);
        this.formSaveMsg.set(null);
    }
}