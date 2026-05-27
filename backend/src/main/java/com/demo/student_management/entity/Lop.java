package com.demo.student_management.entity;

import com.fasterxml.jackson.annotation.JsonIgnore;
import jakarta.persistence.*;
import lombok.*;

import java.util.ArrayList;
import java.util.List;

@Entity
@Table(name = "LOP")
@Getter
@Setter
@NoArgsConstructor
@AllArgsConstructor
@Builder
@EqualsAndHashCode(onlyExplicitlyIncluded = true)
public class Lop {

    @Id
    @Column(name = "ID_LOP", columnDefinition = "char(5)", nullable = false)
    @EqualsAndHashCode.Include
    private String idLop;

    @Column(name = "TEN_LOP", length = 10, nullable = false)
    private String tenLop;

    @Column(name = "SI_SO", nullable = false)
    @Builder.Default
    private Integer siSo = 0;

    @OneToMany(mappedBy = "lop", fetch = FetchType.LAZY)
    @JsonIgnore
    @Builder.Default
    private List<HocSinh> hocSinhs = new ArrayList<>();

    @OneToMany(mappedBy = "lop", fetch = FetchType.LAZY)
    @JsonIgnore
    @Builder.Default
    private List<GiaoVien> giaoViens = new ArrayList<>();
}